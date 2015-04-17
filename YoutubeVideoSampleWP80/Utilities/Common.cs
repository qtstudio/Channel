using System;
using Windows.UI;
using Newtonsoft.Json;

namespace YoutubeVideoSampleWP80.Utilities
{
    public static class Common
    {
        public static T ToObject<T>(this string sourceJson)
        {
            return JsonConvert.DeserializeObject<T>(sourceJson);
        }

        public static Color ToColor(this string str)
        {
            var colorInt = Convert.ToInt32(str, 16);
            var colorA = (byte)(colorInt >> 24);
            var colorR = (byte)(colorInt >> 16);
            var colorG = (byte)(colorInt >> 8);
            var colorB = (byte)(colorInt);
            return Color.FromArgb(colorA, colorR, colorG, colorB);
        }
    }
}
