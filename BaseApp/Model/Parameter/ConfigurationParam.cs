using System.Collections.Generic;

namespace BaseApp.Model.Parameter
{
    class ConfigurationParam
    {
        public string AppName { get; set; }
        public string Description { get; set; }
        public string PublisherName { get; set; }
        public List<ChannelParam> Channel { get; set; }
        public ColorSchemeParam ColorScheme { get; set; }
    }
}