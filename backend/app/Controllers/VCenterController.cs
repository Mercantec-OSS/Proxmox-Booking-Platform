[ApiController]
[Route("vcenter")]
public class VCenterController(Context context, ActivityService activityService, UserSession session, VCenterService vCenterService) : ControllerBase
{
    private readonly VCenterService _vCenterService = vCenterService;
    private readonly EsxiHostService _esxiHostService = new(context);
    private readonly ActivityService _activityService = activityService;

    [HttpGet("{id}")]
    public async Task<ActionResult<VCenterGetDto>> Get(int id)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        VCenter? vCenter = await _vCenterService.GetByIdAsync(id);

        if (vCenter == null)
        {
            return NotFound(ResponseMessage.GetVCenterNotFound());
        }

        return Ok(vCenter.MakeGetDto());
    }

    [HttpGet("vcenter-info")]
    public async Task<ActionResult> GetVcenterInfo()
    {
        VCenterInfoDTO? vCenterInfo = await _vCenterService.GetVcenterInfoAsync();

        if (vCenterInfo == null)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Vcenter info was not found"));
        }

        return Ok(vCenterInfo);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<VCenterGetDto>>> GetAll()
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<VCenter> vCenters = await _vCenterService.GetAllAsync();
        return Ok(vCenters.ConvertAll(v => v.MakeGetDto()));
    }

    [HttpGet("all/available")]
    public async Task<ActionResult<List<VCenterGetDto>>> GetAllAvailable()
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Moderator
        );

        List<VCenter> vCenters = await _vCenterService.GetAvailableAsync();
        return Ok(vCenters.ConvertAll(v => v.MakeGetDto()));
    }

    [HttpPost("create")]
    [ProducesResponseType(201)]
    public async Task<ActionResult> Create(VCenterCreateDto dtoObj)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin
        );

        if (dtoObj == null)
        {
            return BadRequest(ResponseMessage.GetVCenterNotValid());
        }

        dtoObj.Ip = dtoObj.Ip.Trim();
        dtoObj.UserName = dtoObj.UserName.Trim();
        dtoObj.Password = dtoObj.Password.Trim();

        VCenter? dbVcenter = await _vCenterService.GetByIpAsync(dtoObj.Ip);

        if (dbVcenter != null)
        {
            return BadRequest(ResponseMessage.GetVCenterAlreadyExists());
        }

        if (string.IsNullOrEmpty(dtoObj.Ip))
            return BadRequest(ResponseMessage.GetIpIsEmpty());

        if (string.IsNullOrEmpty(dtoObj.UserName) || string.IsNullOrEmpty(dtoObj.Password))
            return BadRequest(ResponseMessage.GetUserDetailsEmpty());

        VCenter vcenter = new()
        {
            Ip = dtoObj.Ip,
            UserName = dtoObj.UserName,
            Password = dtoObj.Password,
            DatacenterName = dtoObj.DatacenterName,
            ClusterName = dtoObj.ClusterName,
            JsonConfig = dtoObj.JsonConfig,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _vCenterService.CreateAsync(vcenter);
        _activityService.CreateActivity(user.Id, vcenter.Id, Activity.ActivityEvent.Create, Activity.ActivityType.VCenter);

        return Ok("VCenter created.");
    }

    [HttpPut("update")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> Update(VCenterUpdateDto dtoObj)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin
        );

        if (dtoObj == null)
        {
            return BadRequest(ResponseMessage.GetVCenterNotValid());
        }

        VCenter? usedIpVcenter = await _vCenterService.GetByIpAsync(dtoObj.Ip);
        
        if (usedIpVcenter != null && usedIpVcenter.Id != dtoObj.Id)
        {
            return BadRequest(ResponseMessage.GetVCenterAlreadyExists());
        }

        VCenter? vcenter = await _vCenterService.GetByIdAsync(dtoObj.Id);
        
        if (vcenter == null)
        {
            return NotFound(ResponseMessage.GetVCenterNotFound());
        }

        dtoObj.Ip = dtoObj.Ip.Trim();
        dtoObj.UserName = dtoObj.UserName.Trim();
        dtoObj.Password = dtoObj.Password.Trim();

        if (string.IsNullOrEmpty(dtoObj.Ip))
            return BadRequest(ResponseMessage.GetIpIsEmpty());

        if (string.IsNullOrEmpty(dtoObj.UserName) || string.IsNullOrEmpty(dtoObj.Password))
            return BadRequest(ResponseMessage.GetUserDetailsEmpty());

        vcenter.Ip = dtoObj.Ip;
        vcenter.UserName = dtoObj.UserName;
        vcenter.Password = dtoObj.Password;
        vcenter.DatacenterName = dtoObj.DatacenterName;
        vcenter.ClusterName = dtoObj.ClusterName;
        vcenter.JsonConfig = dtoObj.JsonConfig;
        vcenter.UpdatedAt = DateTime.UtcNow;

        await _vCenterService.UpdateAsync(vcenter);
        _activityService.CreateActivity(user.Id, vcenter.Id, Activity.ActivityEvent.Update, Activity.ActivityType.VCenter);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin
        );

        VCenter? vcenter = await _vCenterService.GetByIdAsync(id);

        if (vcenter == null)
        {
            return NotFound(ResponseMessage.GetVCenterNotFound());
        }

        List<EsxiHost> hosts = await _esxiHostService.GetByVcenterAsync(vcenter.Id);

        foreach (EsxiHost host in hosts)
        {
            await _esxiHostService.DeleteAsync(host);
        }

        await _vCenterService.DeleteAsync(vcenter);
        _activityService.CreateActivity(user.Id, vcenter.Id, Activity.ActivityEvent.Delete, Activity.ActivityType.VCenter);

        return NoContent();
    }
}