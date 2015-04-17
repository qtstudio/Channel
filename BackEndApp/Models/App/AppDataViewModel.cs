namespace BackendApp.Models.App
{
    public class AppDataViewModel
    {
        public AppDataViewModel()
        {
            AppSharedViewModel = new AppSharedViewModel();
        }
        public AppSharedViewModel AppSharedViewModel { get; set; }
    }
}
