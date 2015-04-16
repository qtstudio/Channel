using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackEndApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnBuildAllApp(object sender, RoutedEventArgs e)
        {
            var si = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = "cmd.exe",
                    Arguments =
                        "/K cd C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC & vcvarsall.bat & CD E:\\YoutubeVideoSampleWP80\\BaseApp.csproj  /p:Configuration=Release"
                }
            };
            si.Start();
            si.Close();
        }
    }
}
