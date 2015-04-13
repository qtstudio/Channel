using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YoutubeVideoSampleWP80.Model
{
    public class Configuration
    {
        public Configuration() { }

        public Configuration(string jsonConfigFile)
        {
        }
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }
        [JsonProperty(PropertyName = "maxResult")]
        public int MaxResult { get; set; }
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }
        [JsonProperty(PropertyName = "orderBy")]
        public string OrderBy { get; set; }
        [JsonProperty(PropertyName = "typeData")]
        public string TypeData { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        public static Configuration CreateConfig(string sourceJson)
        {
            return JsonConvert.DeserializeObject<Configuration>(sourceJson);
        }
    }
}
