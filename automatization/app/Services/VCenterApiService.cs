public class VCenterApiService
{
    private HttpClient _client;
    private readonly Config _config;

    public VCenterApiService(Config config)
    {
        _config = config;
        _client = InitHttpClient();
        Login().Wait();
    }

    private HttpClient InitHttpClient() {
        // Ignore SSL certificate
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        HttpClient client = new HttpClient(handler);

        return client;
    }

    private async System.Threading.Tasks.Task Login() {
        // Add authorization header to client
        var credentials = $"{_config.VM_VCENTER_USER}:{_config.VM_VCENTER_PASSWORD}";
        var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

        // Get and prepare access token
        HttpResponseMessage response = await _client.PostAsync($"https://{_config.VM_VCENTER_IP}/api/session", null);
        string responseBody = await response.Content.ReadAsStringAsync();
        string token = responseBody.Replace("\"", "");

        // set session token to header
        _client.DefaultRequestHeaders.Add("vmware-api-session-id", token);
    }

    public async Task<ResponseVCenterVmDTO?> SearchVmByNameAsync(string vmName) {
        var vms = await _client.GetFromJsonAsync<List<ResponseVCenterVmDTO>>($"https://{_config.VM_VCENTER_IP}/api/vcenter/vm?names={vmName}") ?? new List<ResponseVCenterVmDTO>();
        return vms.FirstOrDefault();
    }

    public async Task<ResponseVCenterTicketDTO> GetVmConnectionUriAsync(string vmName)
    {
        var selectedVmName = await SearchVmByNameAsync(vmName);
        if (selectedVmName == null)
        {
            return new ResponseVCenterTicketDTO();
        }

        var data = new 
        {
            type = "WEBMKS"
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var connectionUri = await _client.PostAsync($"https://{_config.VM_VCENTER_IP}/api/vcenter/vm/{selectedVmName.InternName}/console/tickets", content);
        ResponseVCenterTicketDTO ticket = await connectionUri.Content.ReadFromJsonAsync<ResponseVCenterTicketDTO>() ?? new ResponseVCenterTicketDTO();
        ticket.VcenterIp = _config.VM_VCENTER_IP;

        return ticket;
    }

}