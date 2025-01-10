[ApiController]
[Route("extention-request")]
public class VmBookingExtentionController(
    UserSession session,
    VmBookingExtentionRepository vmBookingExtentionRepository,
    VmBookingRepository vmBookingRepository
    ) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> CreateBookingExtention(VmBookingExtentionCreateDto createDto)
    {
        session.GetIfRoles(
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        List<VmBookingExtention> request = await vmBookingExtentionRepository.GetListByBookingId(createDto.BookingId);
        bool allAccepted = request.All(r => r.IsAccepted);
        if (request.Count == 0)
        {
            allAccepted = true;
        }
        
        if (request.Count > 0 && !allAccepted)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Only one active extention request per booking."));
        }

        VmBooking? booking = await vmBookingRepository.GetByIdAsync(createDto.BookingId);
        
        if (booking == null) {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        VmBookingExtention extention = new()
        {
            OwnerId = booking.OwnerId,
            AssignedId = booking.AssignedId,
            BookingId = booking.Id,
            Message = createDto.Message,
            IsAccepted = false,
            NewExpiredAt = createDto.NewExpiringAt,
        };

        await vmBookingExtentionRepository.CreateAsync(extention);

        return Ok(true);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<VmBookingExtentionGetDto>>> GetAll()
    {
        User user = session.GetIfRoles(
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        List<VmBookingExtention> extentions = new();
        
        if (session.IsAdmin())
        {
            List<VmBookingExtention> selectedExtentions = await vmBookingExtentionRepository.GetAllAsync();
            extentions.AddRange(selectedExtentions);
        }

        else if (session.IsTeacher())
        {
            List<VmBookingExtention> ownExtentions = await vmBookingExtentionRepository.GetByOwnerIdAsync(user.Id);
            List<VmBookingExtention> assignedExtentions = await vmBookingExtentionRepository.GetByAssignedIdAsync(user.Id);
            extentions.AddRange(ownExtentions);
            extentions.AddRange(assignedExtentions);
        }

        else if (session.IsStudent())
        {
            List<VmBookingExtention> ownExtentions = await vmBookingExtentionRepository.GetByOwnerIdAsync(user.Id);
            extentions.AddRange(ownExtentions);
        }

        return Ok(extentions.ConvertAll(e => e.MakeGetDTO()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSingle(int id)
    {
        User user = session.GetIfAuthenticated();

        VmBookingExtention? extention = await vmBookingExtentionRepository.GetByIdAsync(id);
        
        if (extention == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if ((session.IsTeacher() || session.IsStudent()) && user.Id != extention.AssignedId)
        {
            return Unauthorized(ResponseMessage.GetUserUnauthorized());
        }

        return Ok(extention.MakeGetDTO());
    }

    [HttpGet("get/assinged-to/{id}")]
    public async Task<ActionResult> GetAllAssignedToId(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<VmBookingExtention>? extentions = await vmBookingExtentionRepository.GetByAssignedIdAsync(id);

        if (extentions == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(extentions.ConvertAll(e => e.MakeGetDTO()));
    }

    [HttpGet("get/owner/{id}")]
    public async Task<ActionResult> GetAllOwnerBookings(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<VmBookingExtention>? bookings = await vmBookingExtentionRepository.GetByOwnerIdAsync(id);

        if (bookings == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(bookings.ConvertAll(e => e.MakeGetDTO()));
    }

    [HttpPut("accept-extention/{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> AcceptBooking(int id)
    {
       session.GetIfRoles
       (
           Models.User.UserRoles.Admin,
           Models.User.UserRoles.Teacher,
           Models.User.UserRoles.Moderator
       );

        VmBookingExtention? bookingExt = await vmBookingExtentionRepository.GetByIdAsync(id);
        
        if (bookingExt == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        VmBooking? booking = await vmBookingRepository.GetByIdAsync(bookingExt.BookingId);
        
        if (booking == null)
        {
            return BadRequest(ResponseMessage.GetBookingNotFound());
        }

        bookingExt.IsAccepted = true;
        booking.ExpiredAt = bookingExt.NewExpiredAt;

        await vmBookingExtentionRepository.UpdateAsync(bookingExt);
        await vmBookingRepository.UpdateAsync(booking);

        return NoContent();
    }

    [HttpPut("update")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Update(VmBookingUpdateDto updateDTO)
    {
        session.GetIfRoles
        (
           Models.User.UserRoles.Admin,
           Models.User.UserRoles.Teacher,
           Models.User.UserRoles.Moderator
        );

        VmBookingExtention? booking = await vmBookingExtentionRepository.GetByIdAsync(updateDTO.Id);
        
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        booking.UpdatedAt = DateTime.UtcNow;
        booking.ExpiredAt = updateDTO.NewExpiringDate;
        booking.IsAccepted = updateDTO.IsAccepted;

        await vmBookingExtentionRepository.UpdateAsync(booking);

        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Delete(int id)
    {
        session.GetIfRoles
        (
           Models.User.UserRoles.Admin,
           Models.User.UserRoles.Teacher,
           Models.User.UserRoles.Moderator
        );

        VmBookingExtention? booking = await vmBookingExtentionRepository.GetByIdAsync(id);
        
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        await vmBookingExtentionRepository.DeleteAsync(booking);

        return NoContent();
    }
}
