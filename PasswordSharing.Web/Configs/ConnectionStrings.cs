using Newtonsoft.Json;

namespace PasswordSharing.Web.Configs
{
    public class ConnectionStrings
    {
        [JsonProperty("redis", Required = Required.Always)]
        public RedisConfig Redis { get; set; }
    }
}
