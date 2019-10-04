using Newtonsoft.Json;

namespace PasswordSharing.Web.Configs
{
    public class ApplicationConfig
    {
        [JsonProperty("connectionStrings", Required = Required.Always)]
        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
