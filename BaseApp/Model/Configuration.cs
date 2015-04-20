using System.Collections.Generic;
using System.Linq;
using BaseApp.Model.Parameter;
using BaseApp.Model.ViewModel;
using BaseApp.Utilities;

namespace BaseApp.Model
{
    public class Configuration
    {
        public InterfaceViewModel InterfaceViewModel { get; set; }
        public AppInfoViewModel AppInfoViewModel { get; set; }
        public List<ChannelViewModel> ChannelViewModel { get; set; }

        /*[JsonProperty(PropertyName = "index")]
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
        public string Query { get; set; }*/

        public static Configuration Parse(string jsonStr)
        {
            var param = jsonStr.ToObject<ConfigurationParam>();

            if (param == null)
                return null;

            return new Configuration
            {
                AppInfoViewModel = new AppInfoViewModel
                {
                    Name = param.AppName,
                    Description = param.Description,
                    PublisherName = param.
                },
                ChannelViewModel = param.Channel.Select(o => new ChannelViewModel
                {
                    ChannelId = o.ChannelId,
                    Token = o.Token,
                    TypeData = o.TypeData
                }).ToList(),
                InterfaceViewModel = new InterfaceViewModel
                {
                    ColorSchemeViewModel = new ColorSchemeViewModel
                    {
                        DarkColor = param.ColorScheme.DarkColor.ToColor(),
                        LightColor = param.ColorScheme.LightColor.ToColor(),
                        DarkTextColor = param.ColorScheme.DarkTextColor.ToColor(),
                        LightTextColor = param.ColorScheme.LightTextColor.ToColor()
                    }
                }
            };
        }
    }
}
