[ApiController]
[Route("esxi-host")]
public class EsxiController(Context context, ActivityService activityService, UserSession session, VCenterService vCenterService) : ControllerBase
{
    private readonly VCenterService _vCenterService = vCenterService;
    private readonly EsxiHostService _esxiHostService = new(context);
    private readonly ClusterBookingService _bookingService = new(context);
    private readonly ActivityService _activityService = activityService;

    [HttpGet("{id}")]
    public async Task<ActionResult<EsxiHostGetDto>> Get(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        EsxiHost? host = await _esxiHostService.GetByIdAsync(id);

        if (host == null) {
            return NotFound(ResponseMessage.GetEsxiHostNotFound());
        }

        return Ok(host.MakeGetDto());
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<EsxiHostGetDto>>> GetAll()
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<EsxiHost> hosts = await _esxiHostService.GetAllAsync();
        return Ok(hosts.ConvertAll(h => h.MakeGetDto()));
    }

    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> Create(EsxiHostCreateDto dtoObj)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);
     
        if (dtoObj == null) {
            return BadRequest(ResponseMessage.GetHostNotValid());
        }

        dtoObj.Ip = dtoObj.Ip.Trim();
        dtoObj.UserName = dtoObj.UserName.Trim();
        dtoObj.Password = dtoObj.Password.Trim();

        if (string.IsNullOrEmpty(dtoObj.Ip)) {
            return BadRequest(ResponseMessage.GetIpIsEmpty());
        }

        if (string.IsNullOrEmpty(dtoObj.UserName) || string.IsNullOrEmpty(dtoObj.Password)) {
            return BadRequest(ResponseMessage.GetUserDetailsEmpty());
        }

        EsxiHost? existHost = await _esxiHostService.GetByIpAsync(dtoObj.Ip);
        
        if (existHost != null) {
            return BadRequest(ResponseMessage.GetHostAlreadyExists());
        }
        
        VCenter? vCenter = await _vCenterService.GetByIdAsync(dtoObj.VCenterId);

        if (vCenter == null)
        {
            return NotFound(ResponseMessage.GetSelectedVCenterNotFound());
        }
        
        EsxiHost host = new()
        {
            Ip = dtoObj.Ip,
            VCenterId = dtoObj.VCenterId,
            UserName = dtoObj.UserName,
            Password = dtoObj.Password,
            DatastoreName = dtoObj.DatastoreName,
            NetworkName = dtoObj.NetworkName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _esxiHostService.CreateAsync(host);
        return Ok("EsxiHost created.");
    }

    [HttpPut("update")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Update(EsxiHostUpdateDto dtoObj)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);

        if (dtoObj == null) {
            return BadRequest(ResponseMessage.GetHostNotValid());
        }

        dtoObj.Ip = dtoObj.Ip.Trim();
        dtoObj.UserName = dtoObj.UserName.Trim();
        dtoObj.Password = dtoObj.Password.Trim();

        if (string.IsNullOrEmpty(dtoObj.Ip)) {
            return BadRequest(ResponseMessage.GetIpIsEmpty());
        }

        if (string.IsNullOrEmpty(dtoObj.UserName) || string.IsNullOrEmpty(dtoObj.Password)) {
            return BadRequest(ResponseMessage.GetUserDetailsEmpty());
        }

        EsxiHost? usedIpHost = await _esxiHostService.GetByIpAsync(dtoObj.Ip);

        if (usedIpHost != null && usedIpHost.Id != dtoObj.Id) {
            return BadRequest(ResponseMessage.GetHostAlreadyExists());
        }

        EsxiHost? existHost = await _esxiHostService.GetByIdAsync(dtoObj.Id);

        if (existHost == null) {
            return NotFound(ResponseMessage.GetEsxiHostNotFound());
        }

        VCenter? vCenter = await _vCenterService.GetByIdAsync(dtoObj.VCenterId);

        if (vCenter == null) {
            return NotFound(ResponseMessage.GetSelectedVCenterNotFound());
        }

        existHost.Ip = dtoObj.Ip;
        existHost.VCenterId = dtoObj.VCenterId;
        existHost.UserName = dtoObj.UserName;
        existHost.Password = dtoObj.Password;
        existHost.DatastoreName = dtoObj.DatastoreName;
        existHost.NetworkName = dtoObj.NetworkName;

        await _esxiHostService.UpdateAsync(existHost);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);

        EsxiHost? host = await _esxiHostService.GetByIdAsync(id);

        if (host == null) {
            return NotFound(ResponseMessage.GetEsxiHostNotFound());
        } 

        await _esxiHostService.DeleteAsync(host);
        return NoContent();
    }
}