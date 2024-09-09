[ApiController]
[Route("extention-request")]
public class VmBookingExtentionController(Context context, ActivityService activityService, UserSession session) : ControllerBase
{
    private readonly VmBookingExtentionService _vmBookingExtentionService = new(context);
    private readonly UserService _userService = new(context);
    private readonly VmBookingService _vmBookingService = new(context);
    private readonly ActivityService _activityService = activityService;

    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> CreateBookingExtention(VmBookingExtentionCreateDto createDto)
    {
        session.GetIfRoles(
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        List<VmBookingExtention> request = await _vmBookingExtentionService.GetListByBookingId(createDto.BookingId);
        bool allAccepted = request.All(r => r.IsAccepted);
        if (request.Count == 0)
        {
            allAccepted = true;
        }
        
        if (request.Count > 0 && !allAccepted)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Only one active extention request per booking."));
        }

        VmBooking? booking = await _vmBookingService.GetByIdAsync(createDto.BookingId);
        
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

        await _vmBookingExtentionService.CreateAsync(extention);

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
            List<VmBookingExtention> selectedExtentions = await _vmBookingExtentionService.GetAllAsync();
            extentions.AddRange(selectedExtentions);
        }

        else if (session.IsTeacher())
        {
            List<VmBookingExtention> ownExtentions = await _vmBookingExtentionService.GetByOwnerIdAsync(user.Id);
            List<VmBookingExtention> assignedExtentions = await _vmBookingExtentionService.GetByAssignedIdAsync(user.Id);
            extentions.AddRange(ownExtentions);
            extentions.AddRange(assignedExtentions);
        }

        else if (session.IsStudent())
        {
            List<VmBookingExtention> ownExtentions = await _vmBookingExtentionService.GetByOwnerIdAsync(user.Id);
            extentions.AddRange(ownExtentions);
        }

        return Ok(extentions.ConvertAll(e => e.MakeGetDTO()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetSingle(int id)
    {
        User user = session.GetIfAuthenticated();

        VmBookingExtention? extention = await _vmBookingExtentionService.GetByIdAsync(id);
        
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

        List<VmBookingExtention>? extentions = await _vmBookingExtentionService.GetByAssignedIdAsync(id);

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

        List<VmBookingExtention>? bookings = await _vmBookingExtentionService.GetByOwnerIdAsync(id);

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

        VmBookingExtention? bookingExt = await _vmBookingExtentionService.GetByIdAsync(id);
        
        if (bookingExt == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        VmBooking? booking = await _vmBookingService.GetByIdAsync(bookingExt.BookingId);
        
        if (booking == null)
        {
            return BadRequest(ResponseMessage.GetBookingNotFound());
        }

        bookingExt.IsAccepted = true;
        booking.ExpiredAt = bookingExt.NewExpiredAt;

        await _vmBookingExtentionService.UpdateAsync(bookingExt);
        await _vmBookingService.UpdateAsync(booking);

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

        VmBookingExtention? booking = await _vmBookingExtentionService.GetByIdAsync(updateDTO.Id);
        
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        booking.UpdatedAt = DateTime.UtcNow;
        booking.ExpiredAt = updateDTO.NewExpiringDate;
        booking.IsAccepted = updateDTO.IsAccepted;

        await _vmBookingExtentionService.UpdateAsync(booking);
        _activityService.CreateActivity(booking.OwnerId, booking.Id, Activity.ActivityEvent.Update, Activity.ActivityType.BookingRequest);

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

        VmBookingExtention? booking = await _vmBookingExtentionService.GetByIdAsync(id);
        
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        await _vmBookingExtentionService.DeleteAsync(booking);
        _activityService.CreateActivity(booking.OwnerId, booking.Id, Activity.ActivityEvent.Delete, Activity.ActivityType.BookingRequest);

        return NoContent();
    }
}
