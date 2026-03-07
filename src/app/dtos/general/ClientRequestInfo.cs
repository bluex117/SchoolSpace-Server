using backend.app.dtos.general;

namespace backend.app.dtos.general
{
    public class ClientRequestInfo
    {
        public string IpAddress { get; set; } = "Unknown";
        public string DeviceType { get; set; } = "Unknown";
        public string ClientName { get; set; } = "Unknown";
        public UserIdentityPayload? UserPayload { get; set; }
    }
}
