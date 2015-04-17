using System.Windows;
using BackendApp.Models.App;

namespace BackendApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ChannelDBEntities _channelDbEntities = new ChannelDBEntities();
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
                        "/K cd C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC & vcvarsall.bat  & CD C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319 & MSBuild C:\\Users\\nvtan_000\\Documents\\GitHub\\Channel\\YoutubeVideoSampleWP80\\BaseApp.csproj  /p:Configuration=Release"
                }
            };
            si.Start();
            si.Close();
        }

        private void BtnCreateApp(object sender, RoutedEventArgs e)
        {
            var viewModel = AppInfoView.DataContext as AppDataViewModel;
            if (viewModel == null) return;


            _channelDbEntities.AppInfoes.Add(new AppInfo
            {
                Name = viewModel.AppSharedViewModel.Name,
                LongDesc = viewModel.AppSharedViewModel.LongDesc,
                ShortDesc = viewModel.AppSharedViewModel.ShortDesc,
                NameOnStore = viewModel.AppSharedViewModel.NameOnStore,
                LinkOnStore = viewModel.AppSharedViewModel.LinkOnStore
            });
            _channelDbEntities.SaveChanges();
            //_qtDatabaseEntities.ChannelAppConfigs.Add(new ChannelAppConfig
            //{
            //    ChannelId = "test",
            //    Index = 1,
            //    MaxResult = 10,
            //    OrderBy = "published",
            //    Query = "",
            //    TypeData = "rss"
            //});
            //_qtDatabaseEntities.SaveChanges();
            //MessageBox.Show("Create successfully!");

            //_qtDatabaseEntities.ChannelAppConfigs.ToList();
        }
    }
}
