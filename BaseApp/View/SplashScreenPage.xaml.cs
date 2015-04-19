using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Threading.Tasks;

namespace BaseApp.View
{
    public partial class SplashScreenPage : PhoneApplicationPage
    {
        public SplashScreenPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));

            base.OnNavigatedTo(e);
        }
    }
}