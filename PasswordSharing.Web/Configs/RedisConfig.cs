using System;
using Newtonsoft.Json;

namespace PasswordSharing.Web.Configs
{
    public class RedisConfig
    {
        [JsonProperty("connectionString", Required = Required.Always)]
        public string ConnectionString { get; set; }

        [JsonProperty("defaultExpiration", Required = Required.Always)]
        public TimeSpan DefaultExpiration { get; set; }
    }
}
