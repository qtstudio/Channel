using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using BaseApp.Resources;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using BaseApp.Model;
using BaseApp.Utilities;

namespace BaseApp.View
{
    public partial class MainPage
    {
        private readonly Configuration _configuration  = ((App)Application.Current).Configuration; 
        private readonly DetectorLongList _detectorLongList;

        private const int _maxResult = 20;
        private string _orderBy = "published";
        private int _index = 1;
        private string _query = "";
        private int _totalResults;
        private int _totalPages = 1;
        private int _crrPage = 1;
        public MainPage()
        {
            InitializeComponent();
            BuildApplicationBar();
            FeedbackOverlay.VisibilityChanged += FeedbackOverlay_VisibilityChanged;
        }
        public InterfaceViewModel InterfaceViewModel
        {
            get { return _configuration.InterfaceViewModel; }
        }

        public AppInfoViewModel AppInfoViewModel
        {
            get { return _configuration.AppInfoViewModel; }
        }

        public ColorSchemeViewModel ColorSchemeViewModel
        {
            get { return InterfaceViewModel.ColorSchemeViewModel; }
        }

        public List<ChannelViewModel> ChannelViewModel
        {
            get { return _configuration.ChannelViewModel; }
        }

        void FeedbackOverlay_VisibilityChanged(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = (FeedbackOverlay.Visibility != Visibility.Visible);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    if (ChannelVideos.ItemsSource != null)
                    {
                        ChannelVideos.ItemsSource.Clear();
                    }

                    await GetDataForList();
                    _totalPages = (int)Math.Ceiling((double)_totalResults / _maxResult);
                }
                else
                {
                    MessageBox.Show("You're not connected to the Internet!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            base.OnNavigatedTo(e);
        }

        private async Task GetDataForList()
        {
            Indicator.Text = AppResources.MainPage_CS_LoadingVideos;
            Indicator.IsIndeterminate = true;

            var channelInfo = ChannelViewModel[0];

            var channelVideos =
                await
                    GetYoutubeChannel("http://gdata.youtube.com/feeds/api/users/" + channelInfo.ChannelId +
                                      "/uploads?alt=" + channelInfo.TypeData + "&v=2&orderby=" + _orderBy + "&start-index=" + _index +
                                      "&max-results=" + _maxResult + (string.IsNullOrEmpty(_query) ? "" : ("&q=" + _query)));
            if (ChannelVideos.ItemsSource != null)
                ChannelVideos.ItemsSource.Clear();
            ChannelVideos.ItemsSource = channelVideos;

            CurrentPageXaml.Text = _crrPage.ToString();

            Indicator.Text = "";
            Indicator.IsIndeterminate = false;
        }

        private async Task<List<YoutubeVideo>> GetYoutubeChannel(string url)
        {
            try
            {
                var client = new HttpClient();
                var feedXml = await client.GetStringAsync(new Uri(url));

                //var atomns = XNamespace.Get("http://www.w3.org/2005/Atom");
                var yt = XNamespace.Get("http://gdata.youtube.com/schemas/2007");
                var openSearch = XNamespace.Get("http://a9.com/-/spec/opensearch/1.1/");
                var media = XNamespace.Get("http://search.yahoo.com/mrss/");
                var gd = XNamespace.Get("http://schemas.google.com/g/2005");

                var xml = XElement.Parse(feedXml);
                var channel = xml.Element("channel");
                var items = channel.Elements("item");
                var videosList = new List<YoutubeVideo>();
                _totalResults = int.Parse(channel.Element(openSearch + "totalResults").Value);
                foreach (var item in items)
                {
                    var mediaGroup = item.Element(media + "group");
                    var video = new YoutubeVideo
                    {
                        YoutubeLink = new Uri(item.Element("link").Value),
                        Title = item.Element("title").Value,
                        PubDate = DateTime.Parse(item.Element("pubDate").Value),
                        Duration = new TimeSpan(0, 0, 0, (int)mediaGroup.Element(yt + "duration").Attribute("seconds")),
                        Likes = (int)item.Element(yt + "rating").Attribute("numLikes"),
                        ViewCount = (int)item.Element(yt + "statistics").Attribute("viewCount"),
                        Thumbnail = new Uri(mediaGroup.Elements(media + "thumbnail").FirstOrDefault(o => o.Attribute(yt + "name").Value == "mqdefault").Attribute("url").Value),
                        Rating = (float)item.Element(gd + "rating").Attribute("average")
                    };

                    var bm = new BitmapImage(video.Thumbnail) { CreateOptions = BitmapCreateOptions.None };
                    bm.ImageOpened += (s, e) =>
                    {
                        if (video.BlurBgSource != null) return;

                        var wb = new WriteableBitmap((BitmapImage)s);
                        wb.BoxBlur(23);
                        video.BlurBgSource = wb;
                        video.BlurBgSource.Invalidate();
                    };

                    var a = video.YoutubeLink.ToString().Remove(0, 31);
                    video.Id = a.Substring(0, 11);
                    videosList.Add(video);
                }
                return videosList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        //After selecting a video, navigate to the VideoPage
        private void VideosList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var video = e.AddedItems[0] as YoutubeVideo;
            if (video != null)
                NavigationService.Navigate(new Uri("/View/VideoPage.xaml?videoId=" + video.Id, UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Terminate();
            base.OnBackKeyPress(e);
        }

        public async void NextClick(object sender, EventArgs e)
        {
            _index += _maxResult;
            if (_index >= _totalResults)
            {
                _index -= _maxResult;
                return;
            }
            _crrPage = _index / _maxResult + 1;
            await GetDataForList();
        }
        public async void PreviousClick(object sender, EventArgs e)
        {
            _index -= _maxResult;
            if (_index < 1)
            {
                _index += _maxResult;
                return;
            }
            _crrPage = _index / _maxResult + 1;
            await GetDataForList();
        }

        public void SearchClick(object sender, EventArgs e)
        {
            var input = new InputPrompt();
            input.Completed += input_Completed;
            input.Title = "Search";
            input.Value = HttpUtility.UrlDecode(_query);
            input.Show();
        }

        private async void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            _index = 1;
            _crrPage = _index / _maxResult + 1;
            _query = HttpUtility.UrlEncode(e.Result);
            await GetDataForList();
        }

        public async void PublishedClick(object sender, EventArgs e)
        {
            _index = 1;
            _crrPage = _index / _maxResult + 1;
            _orderBy = OrderByType.published.ToString();
            await GetDataForList();
        }

        public async void RatingClick(object sender, EventArgs e)
        {
            _index = 1;
            _crrPage = _index / _maxResult + 1;
            _orderBy = OrderByType.rating.ToString();
            await GetDataForList();
        }

        public async void ViewCountClick(object sender, EventArgs e)
        {
            _index = 1;
            _crrPage = _index / _maxResult + 1;
            _orderBy = OrderByType.viewCount.ToString();
            await GetDataForList();
        }

        public void RateAndReviewClick(object sender, EventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        public async void ReloadClick(object sender, EventArgs e)
        {
            await GetDataForList();
        }

        public void FeedbackClick(object sender, EventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
            {
                Subject = "Feedback of " + AppInfoViewModel.Name,
                Body = "",
                To = "joy.entertainment@outlook.com"
            };
            emailComposeTask.Show();
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var date = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/Icons/published.png", UriKind.Relative),
                Text = AppResources.MainPage_AppBar_Date,
            };
            date.Click += PublishedClick;

            var viewCount = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/Icons/viewCount.png", UriKind.Relative),
                Text = AppResources.MainPage_AppBar_Views,
            };
            viewCount.Click += ViewCountClick;


            var rating = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/Icons/rating.png", UriKind.Relative),
                Text = AppResources.MainPage_AppBar_Likes,
            };
            rating.Click += RatingClick;

            var reload = new ApplicationBarMenuItem
            {
                Text = AppResources.MainPage_AppMenu_Reload
            };
            reload.Click += ReloadClick;

            var rateAndReview = new ApplicationBarMenuItem
            {
                Text = AppResources.MainPage_AppMenu_RateReview
            };
            rateAndReview.Click += RateAndReviewClick;

            var feedback = new ApplicationBarMenuItem
            {
                Text = AppResources.MainPage_AppMenu_Feedback
            };
            feedback.Click += FeedbackClick;

            ApplicationBar.Buttons.Add(date);
            ApplicationBar.Buttons.Add(viewCount);
            ApplicationBar.Buttons.Add(rating);

            ApplicationBar.MenuItems.Add(reload);
            ApplicationBar.MenuItems.Add(rateAndReview);
            ApplicationBar.MenuItems.Add(feedback);
        }

        public void ShareClick(object sender, RoutedEventArgs e)
        {
            var item =(sender as MenuItem).DataContext as YoutubeVideo;
            
            var shareLinkTask = new ShareLinkTask
            {
                Title = "Share link from " + AppInfoViewModel.Name + " app",
                LinkUri = item.YoutubeLink, 
                Message = "Thank you for using " + AppInfoViewModel.Name + " app!"
            };
            shareLinkTask.Show();
        }
    }
}