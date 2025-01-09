[ApiController]
[Route("vm-booking")]
public class VmBookingController(
    Context context, 
    UserSession session, 
    VmBookingScriptService vmBookingScriptService, 
    VmBookingService _vmBookingService,
    EmailService emailService
    ) : ControllerBase
{
    private readonly UserService _userService = new(context);

    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> CreateRequestBooking(VmBookingCreateDto bookingDTO)
    {
        session.GetIfAuthenticated();

        User? ownerUser = await _userService.GetAsync(bookingDTO.OwnerId ?? -1);
        User? assignedToUser = await _userService.GetAsync(bookingDTO.AssignedId ?? -1);
        bool isAccepted = false;

        if (ownerUser == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage($"Owner user id:'{bookingDTO.OwnerId}' dont exist"));
        }

        if (assignedToUser == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage($"Assigned user id:'{bookingDTO.AssignedId}' dont exist"));
        }

        if  (bookingDTO.Type == null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Type is required"));
        }

        // for teacher and admin make accepted booking
        if (session.IsTeacher() || session.IsAdmin())
        {
            isAccepted = true;
        }

        TemplateGetDto? template = TemplateGetDto.MakeGetDTO(bookingDTO.Type);

        VmBooking booking = new()
        {
            Id = 0,
            AssignedId = assignedToUser.Id,
            OwnerId = ownerUser.Id,
            Type = bookingDTO.Type,
            Message = bookingDTO.Message,
            Name = $"{ownerUser.Id}_{Guid.NewGuid()}",
            Login = VmCredentials.GetLoginByTemplate(template),
            Password = VmCredentials.GetPasswordByTemplate(template),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiredAt = bookingDTO.ExpiringAt,
            IsAccepted = isAccepted,
        };

        await _vmBookingService.CreateAsync(booking);

        Email email = Email.GetVmBookingCreate(booking);
        await emailService.SendAsync(email);
        
        // if booking is accepted create vm
        if (isAccepted)
        {
            vmBookingScriptService.Create(booking.Name, booking.Type, Config.VM_ROOT_PASSWORD, booking.Login, booking.Password);
        }

        else
        {
            Email emailAccept = Email.GetVmBookingToAccept(booking);
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
            List<VmBooking> selectedVmBookings = await _vmBookingService.GetByOnwerIdAsync(user.Id);
            bookingsToReturn.AddRange(selectedVmBookings);
        }

        // Selected bookings for teacher
        else if (session.IsTeacher())
        {
            List<VmBooking> allBookings = await _vmBookingService.GetAllAsync();
            List<VmBooking> selectedVmBookings = allBookings.Where(booking => booking.OwnerId == user.Id || booking.AssignedId == user.Id).ToList();
            bookingsToReturn.AddRange(selectedVmBookings);
        }

        // all bookings for admin and moderator
        else
        {
            List<VmBooking> allBookings = await _vmBookingService.GetAllAsync();
            bookingsToReturn.AddRange(allBookings);
        }

        return Ok(bookingsToReturn.ConvertAll(b => b.MakeGetDTO()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSingle(int id)
    {
        User user = session.GetIfAuthenticated();
        VmBooking? booking = await _vmBookingService.GetByIdAsync(id);

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

        List<VmBooking>? bookings = await _vmBookingService.GetByAssignedToIdAsync(id);
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

        List<VmBooking> bookings = await _vmBookingService.GetByOnwerIdAsync(id);
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

        VmBooking? booking = await _vmBookingService.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (session.IsTeacher() && booking.AssignedId != user.Id) {
            return Unauthorized(ResponseMessage.GetErrorMessage("Teacher can accept only own assigned bookings"));
        }

        booking.IsAccepted = true;
        await _vmBookingService.UpdateAsync(booking);

        Email email = Email.GetVmBookingAccepted(booking);
        await emailService.SendAsync(email);
        vmBookingScriptService.Create(booking.Name, booking.Type, Config.VM_ROOT_PASSWORD, booking.Login, booking.Password);

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

        VmBooking? booking = await _vmBookingService.GetByIdAsync(updateDto.Id);
        
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        booking.ExpiredAt = updateDto.NewExpiringDate;
        booking.IsAccepted = updateDto.IsAccepted;

        await _vmBookingService.UpdateAsync(booking);

        Email email = Email.GetVmBookingUpdated(booking);
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

        VmBooking? booking = await _vmBookingService.GetByIdAsync(id);

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
        if (session.IsTeacher() && (booking.OwnerId != user.Id || booking.AssignedId != user.Id)) {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        await _vmBookingService.DeleteAsync(booking);
        vmBookingScriptService.Remove(booking.Name);

        return NoContent();
    }
}