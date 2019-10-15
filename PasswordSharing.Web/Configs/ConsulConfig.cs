using Newtonsoft.Json;

namespace PasswordSharing.Web.Configs
{
    public class ConsulConfig
    {
        [JsonProperty("url", Required = Required.Always)]
        public string Url { get; set; }

        [JsonProperty("serviceId", Required = Required.Always)]
        public string ServiceId { get; set; }

        [JsonProperty("serviceName", Required = Required.Always)]
        public string ServiceName { get; set; }
    }
}
