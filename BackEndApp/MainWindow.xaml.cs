using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                        "/K cd C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\VC & vcvarsall.bat  & CD C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319 & MSBuild C:\\Users\\nvtan_000\\Documents\\GitHub\\Channel\\BaseApp\\BaseApp.csproj  /p:Configuration=Release"
                }
            };
            si.Start();
            si.Close();
        }

        private void BtnCreateApp(object sender, RoutedEventArgs e)
        {
            var viewModel = AppInfoView.DataContext as AppDataViewModel;
            if (viewModel == null || viewModel.AppSharedViewModel == null) return;

            if (_channelDbEntities.AppInfoes.Any(o => o.Name == viewModel.AppSharedViewModel.Name))
            {
                MessageBox.Show("Duplicate name");
                return;
            }

            _channelDbEntities.AppInfoes.Add(new AppInfo
            {
                Name = viewModel.AppSharedViewModel.Name,
                LongDesc = viewModel.AppSharedViewModel.LongDesc,
                ShortDesc = viewModel.AppSharedViewModel.ShortDesc,
                NameOnStore = viewModel.AppSharedViewModel.NameOnStore,
                LinkOnStore = viewModel.AppSharedViewModel.LinkOnStore
            });
            _channelDbEntities.SaveChanges();
            ReloadGrid();
        }

        private void BtnRefreshAppInfoGrid(object sender, RoutedEventArgs e)
        {
            ReloadGrid();
        }

        void ReloadGrid()
        {
            ListBox.ItemsSource = _channelDbEntities.AppInfoes.Select(o => new AppDataGridVo
            {
                Id = o.Id,
                Name = o.Name
            }).ToList();
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;

            var addedItem = e.AddedItems[0] as AppDataGridVo;
            if (addedItem == null) return;

            var selectedEntity = _channelDbEntities.AppInfoes.FirstOrDefault(o => o.Id == addedItem.Id);
            if (selectedEntity == null) return;

            AppInfoView.DataContext = new AppDataViewModel
            {
                AppSharedViewModel = new AppSharedViewModel
                {
                    Id = selectedEntity.Id,
                    Name = selectedEntity.Name,
                    ShortDesc = selectedEntity.ShortDesc,
                    LongDesc = selectedEntity.LongDesc,
                    NameOnStore = selectedEntity.NameOnStore,
                    LinkOnStore = selectedEntity.LinkOnStore
                }
            };
        }

        private void BtnUpdateApp(object sender, RoutedEventArgs e)
        {
            var viewModel = AppInfoView.DataContext as AppDataViewModel;
            if (viewModel == null || viewModel.AppSharedViewModel == null) return;

            var sharedViewModel = viewModel.AppSharedViewModel;

            var entity = _channelDbEntities.AppInfoes.FirstOrDefault(o => o.Id == sharedViewModel.Id);

            if (entity == null)
            {
                MessageBox.Show("App Info is not existed");
                return;
            }

            if (_channelDbEntities.AppInfoes.Any(o => o.Name == sharedViewModel.Name && o.Id != sharedViewModel.Id))
            {
                MessageBox.Show("Duplicate name.");
                return;
            }

            entity.Name = sharedViewModel.Name;
            entity.LongDesc = sharedViewModel.LongDesc;
            entity.ShortDesc = sharedViewModel.ShortDesc;
            entity.NameOnStore = sharedViewModel.NameOnStore;
            entity.LinkOnStore = sharedViewModel.LinkOnStore;

            _channelDbEntities.SaveChanges();

            ReloadGrid();
        }
    }
}
