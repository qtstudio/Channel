using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YoutubeVideoSampleWP80.Utilities
{
    public static class FileHelper
    {
        public static string ReadFile(string filePath)
        {
            var resourceStream = Application.GetResourceStream(new Uri(filePath));
            if (resourceStream == null) return "";

            var myFileStream = resourceStream.Stream;

            if (!myFileStream.CanRead) return "";

            var myStreamReader = new StreamReader(myFileStream);

            return myStreamReader.ReadToEnd();
        }
    }
}
