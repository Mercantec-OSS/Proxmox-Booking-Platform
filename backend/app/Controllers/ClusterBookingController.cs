[ApiController]
[Route("cluster-booking")]
public class ClusterBookingController(Context context, ScriptService scriptService, Config config, UserSession session, VCenterService vCenterService) : ControllerBase
{
    private readonly ClusterBookingService _bookingService = new(context);
    private readonly VCenterService _vcenterService = vCenterService;
    private readonly EsxiHostService _esxiHostService = new(context);
    private readonly EmailService _emailService = new(config);

    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> Create(ClusterBookingCreateDto bookingDTO)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        // retrieve user from db
        User user = session.User;

        // Check if any available vCenters exist
        var availableVCenters = await _vcenterService.GetAvailableAsync();

        if (bookingDTO.AmountStudents < 3)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Amount of students must be at least 3"));
        }

        if (bookingDTO.AmountDays < 1)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Amount of days must be at least 1"));
        }

        if (availableVCenters.Count == 0)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("No available VCenters for booking"));
        }

        // Booking creating
        ClusterBooking booking = new()
        {
            OwnerId = user.Id,
            Owner = user,
            AmountStudents = bookingDTO.AmountStudents,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.Add(TimeSpan.FromDays(bookingDTO.AmountDays))
        };

        await _bookingService.CreateAsync(booking);

        // Book vCenters
        int amountVCenters = (int)(double)(bookingDTO.AmountStudents / 3);
        List<VCenter> selectedVcenters = [];

        if (availableVCenters.Count >= amountVCenters)
            selectedVcenters.AddRange(availableVCenters.Take(amountVCenters));
        else
            selectedVcenters.AddRange(availableVCenters);

        // Set booking ID for selected vCenters
        foreach (VCenter vcenter in selectedVcenters)
        {
            vcenter.BookingId = booking.Id;
            await _vcenterService.UpdateAsync(vcenter);
        }

        // Data preparing to email
        List<EsxiHost> selectedHosts = [];
        foreach (VCenter vcenter in selectedVcenters)
            selectedHosts.AddRange(await _esxiHostService.GetByVcenterAsync(vcenter.Id));

        await scriptService.ResetAndInstallVcenterAsync(selectedVcenters);

        _emailService.SendClusterBookingCreate(booking);

        return Ok(booking.MakeGetDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<ClusterBookingGetDto>>> GetById(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        ClusterBooking? booking = await _bookingService.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(booking.MakeGetDto());
    }

    [HttpGet("owner/{id}")]
    public async Task<ActionResult<List<ClusterBookingGetDto>>> GetByOwnerId(int id)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<ClusterBooking> bookings = await _bookingService.GetByOwnerAsync(id);
        List<ClusterBookingGetDto> result = bookings.ConvertAll(b => b.MakeGetDto()).OrderBy(r => r.CreatedAt).Reverse().ToList();

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ClusterBookingGetDto>>> GetAll()
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<ClusterBooking> bookings = await _bookingService.GetAllAsync();
        List<ClusterBookingGetDto> bookingDtos = bookings.ConvertAll(b => b.MakeGetDto()).OrderBy(r => r.CreatedAt).Reverse().ToList();

        return Ok(bookingDtos);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        ClusterBooking? existingBooking = await _bookingService.GetByIdAsync(id);

        if (existingBooking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // update all vcenters
        List<VCenter> usedVCenters = await _vcenterService.GetByBookingIdAsync(id);

        foreach (VCenter vcenter in usedVCenters)
        {
            vcenter.BookingId = null;
            await _vcenterService.UpdateAsync(vcenter);
        }

        await _bookingService.DeleteAsync(existingBooking);
        await scriptService.ResetClusterBookingAsync(usedVCenters);

        return NoContent();
    }
}
