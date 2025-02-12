public class WebsockifyService {
    public int Start(ProxmoxVncDto vncInfo) {
        int websockifyPort = GetAvailablePort();

        var startInfo = new ProcessStartInfo
        {
            FileName = "timeout",
            Arguments = $"3600 websockify 0.0.0.0:{websockifyPort} 10.134.11.11:{vncInfo.Port} --run-once",
            UseShellExecute = false,
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
        }

        return websockifyPort;
    }

    private int GetAvailablePort()
    {
        int port;
        do
        {
            port = GetRandomPort();
        } while (!IsPortAvailable(port));

        return port;
    }

    private int GetRandomPort()
    {
        Random random = new Random();
        return random.Next(10000, 20000);
    }

    private bool IsPortAvailable(int port)
    {
        var activeConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
        return !activeConnections.Any(endpoint => endpoint.Port == port);
    }
}