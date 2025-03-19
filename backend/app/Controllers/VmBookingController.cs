[ApiController]
[Route("vm-booking")]
public class VmBookingController(
    UserSession session,
    VmService vmService,
    VmBookingRepository vmBookingRepository,
    UserRepository userRepository,
    EmailService emailService
    ) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> CreateRequestBooking(VmBookingCreateDto bookingDTO)
    {
        session.GetIfAuthenticated();

        User? ownerUser = await userRepository.GetAsync(bookingDTO.OwnerId ?? -1);
        User? assignedToUser = await userRepository.GetAsync(bookingDTO.AssignedId ?? -1);
        bool isAccepted = false;

        if (ownerUser == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage($"Owner user id:'{bookingDTO.OwnerId}' dont exist"));
        }

        if (assignedToUser == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage($"Assigned user id:'{bookingDTO.AssignedId}' dont exist"));
        }

        if (bookingDTO.Type == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Type is required"));
        }

        if (bookingDTO.ExpiringAt < DateTime.UtcNow)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Expiring date must be in future"));
        }

        if (bookingDTO.ExpiringAt > DateTime.UtcNow + TimeSpan.FromDays(200))
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Max 200 days allowed for booking"));
        }

        // limitation for students
        if (session.IsStudent())
        {
            List<VmBooking> bookings = await vmBookingRepository.GetByOnwerIdAsync(ownerUser.Id);
            if (bookings.Count >= 5)
            {
                return UnprocessableEntity(ResponseMessage.GetErrorMessage("Max 5 bookings allowed. Delete some to create new or contact teacher"));
            }
        }

        // for teacher and admin make accepted booking
        if (session.IsTeacher() || session.IsAdmin())
        {
            isAccepted = true;
        }

        ProxmoxVmDto templateProxmox = await vmService.GetTemplateByNameAsync(bookingDTO.Type);
        VmBooking booking = new()
        {
            Id = 0,
            AssignedId = assignedToUser.Id,
            OwnerId = ownerUser.Id,
            Type = bookingDTO.Type,
            Message = bookingDTO.Message,
            Name = $"notready--{Helpers.GetRandomNumber()}--{ownerUser.Id}--{templateProxmox.Name.ToLower()}",
            Login = VmCredentials.GetLoginByTemplate(templateProxmox.Tags),
            Password = VmCredentials.GetPasswordByTemplate(templateProxmox.Tags),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiredAt = bookingDTO.ExpiringAt,
            IsAccepted = isAccepted,
        };

        await vmBookingRepository.CreateAsync(booking);

        EmailDto email = EmailDto.GetVmBookingCreate(booking);
        await emailService.SendAsync(email);

        // if booking is accepted create vm
        if (isAccepted)
        {
            _ = vmService.Book(booking.Name, booking.Type, booking.Login, booking.Password);
        }

        else
        {
            EmailDto emailAccept = EmailDto.GetVmBookingToAccept(booking);
            await emailService.SendAsync(emailAccept);

        }

        return Ok(true);
    }

    [HttpGet("all")]
    public async Task<ActionResult> GetAll()
    {
        User user = session.GetIfAuthenticated();
        List<VmBooking> bookingsToReturn = new();

        // All students bookings
        if (session.IsStudent())
        {
            List<VmBooking> selectedVmBookings = await vmBookingRepository.GetByOnwerIdAsync(user.Id);
            bookingsToReturn.AddRange(selectedVmBookings);
        }

        // Selected bookings for teacher
        else if (session.IsTeacher())
        {
            List<VmBooking> allBookings = await vmBookingRepository.GetAllAsync();
            List<VmBooking> selectedVmBookings = allBookings.Where(booking => booking.OwnerId == user.Id || booking.AssignedId == user.Id).ToList();
            bookingsToReturn.AddRange(selectedVmBookings);
        }

        // all bookings for admin and moderator
        else
        {
            List<VmBooking> allBookings = await vmBookingRepository.GetAllAsync();
            bookingsToReturn.AddRange(allBookings);
        }

        return Ok(bookingsToReturn.ConvertAll(b => b.MakeGetDTO()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSingle(int id)
    {
        User user = session.GetIfAuthenticated();
        VmBooking? booking = await vmBookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (session.IsStudent() && booking.OwnerId != user.Id)
        {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        return Ok(booking.MakeGetDTO());
    }

    [HttpGet("assinged/{id}")]
    public async Task<ActionResult> GetAllAssignedToId(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<VmBooking>? bookings = await vmBookingRepository.GetByAssignedToIdAsync(id);
        return Ok(bookings.ConvertAll(b => b.MakeGetDTO()));
    }

    [HttpGet("owner/{id}")]
    public async Task<ActionResult> GetAllOwnerBookings(int id)
    {
        User user = session.GetIfAuthenticated();

        if (session.IsStudent() && user.Id != id)
        {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        List<VmBooking> bookings = await vmBookingRepository.GetByOnwerIdAsync(id);
        return Ok(bookings.ConvertAll(b => b.MakeGetDTO()));
    }

    [HttpPut("accept/{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> AcceptBooking(int id)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        VmBooking? booking = await vmBookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (session.IsTeacher() && booking.AssignedId != user.Id)
        {
            return Unauthorized(ResponseMessage.GetErrorMessage("Teacher can accept only own assigned bookings"));
        }

        booking.IsAccepted = true;
        await vmBookingRepository.UpdateAsync(booking);

        EmailDto email = EmailDto.GetVmBookingAccepted(booking);
        await emailService.SendAsync(email);
        _ = vmService.Book(booking.Name, booking.Type, booking.Login, booking.Password);

        return NoContent();
    }

    [HttpPut("update")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Update(VmBookingUpdateDto updateDto)
    {
        session.GetIfRoles(
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        VmBooking? booking = await vmBookingRepository.GetByIdAsync(updateDto.Id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        booking.ExpiredAt = updateDto.NewExpiringDate;
        booking.IsAccepted = updateDto.IsAccepted;

        await vmBookingRepository.UpdateAsync(booking);

        EmailDto email = EmailDto.GetVmBookingUpdated(booking);
        await emailService.SendAsync(email);

        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Delete(int id)
    {
        User user = session.GetIfRoles(
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        VmBooking? booking = await vmBookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Student can delete only own bookings
        if (session.IsStudent() && booking.OwnerId != user.Id)
        {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        // Teacher can delete only own and assigned to yourself bookings
        if (session.IsTeacher() && (booking.OwnerId != user.Id || booking.AssignedId != user.Id))
        {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        await vmBookingRepository.DeleteAsync(booking);
        _ = vmService.Remove(booking.Name);

        return NoContent();
    }
}