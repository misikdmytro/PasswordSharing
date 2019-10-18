using Newtonsoft.Json;

namespace PasswordSharing.Web.Configs
{
    public class ApplicationConfig
    {
        [JsonProperty("connectionStrings", Required = Required.Always)]
        public ConnectionStrings ConnectionStrings { get; set; }

        [JsonProperty("consul", Required = Required.Default)]
        public ConsulConfig Consul { get; set; }

        [JsonProperty("fabio", Required = Required.Default)]
        public FabioConfig Fabio { get; set; }
    }
}
