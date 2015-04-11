using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml.Linq;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using YoutubeVideoSampleWP80.Model;
using YoutubeVideoSampleWP80.Utilities;

namespace YoutubeVideoSampleWP80.View
{
    public partial class MainPage
    {
        public int Index = 1;
        public int MaxResults = 10;
        public const string YoutubeChannel = "JackyPhanChanel";
        public string OrderBy = OrderByType.published.ToString();
        public const string TypeData = "rss";
        public const string Token = "AIzaSyDNRQKpwuOIdorBHisGMUgakC_hwvjD8l8";
        public int TotalPage = 1;
        public int TotalResults;
        public int CurrentPage = 1;
        public string Query = "";
        public MainPage()
        {
            InitializeComponent();
            FeedbackOverlay.VisibilityChanged += FeedbackOverlay_VisibilityChanged;

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
                    await GetDataForList();

                    TotalPage = (int)Math.Ceiling((double)TotalResults / MaxResults);
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
            ChannelVideos.Visibility = Visibility.Collapsed;
            ChannelProgress.Visibility = Visibility.Visible;

            var channelVideos =
                await
                    GetYoutubeChannel("http://gdata.youtube.com/feeds/api/users/" + YoutubeChannel +
                                      "/uploads?alt=" + TypeData + "&v=2&orderby=" + OrderBy + "&start-index=" + Index +
                                      "&max-results=" + MaxResults + (string.IsNullOrEmpty(Query) ? "" : ("&q=" + Query)));
            if (ChannelVideos.ItemsSource != null)
                ChannelVideos.ItemsSource.Clear();
            ChannelVideos.ItemsSource = channelVideos;

            //CurrentPageXaml.Text = CurrentPage.ToString();
            ChannelVideos.Visibility = Visibility.Visible;
            ChannelProgress.Visibility = Visibility.Collapsed;
        }

        private async Task<List<YoutubeVideo>> GetYoutubeChannel(string url)
        {
            try
            {
                var client = new HttpClient();
                var feedXml = await client.GetStringAsync(new Uri(url));

                var atomns = XNamespace.Get("http://www.w3.org/2005/Atom");
                var yt = XNamespace.Get("http://gdata.youtube.com/schemas/2007");
                var openSearch = XNamespace.Get("http://a9.com/-/spec/opensearch/1.1/");
                var media = XNamespace.Get("http://search.yahoo.com/mrss/");
                var gd = XNamespace.Get("http://schemas.google.com/g/2005");

                var xml = XElement.Parse(feedXml);
                var channel = xml.Element("channel");
                var items = channel.Elements("item");
                var videosList = new List<YoutubeVideo>();
                TotalResults = int.Parse(channel.Element(openSearch + "totalResults").Value);
                foreach (var item in items)
                {
                    var mediaGroup = item.Element(media + "group");
                    var video = new YoutubeVideo
                    {
                        YoutubeLink = new Uri(item.Element("link").Value),
                        Title = item.Element("title").Value,
                        PubDate = DateTime.Parse(item.Element("pubDate").Value),
                        Duration = new TimeSpan(0,0,0,(int)mediaGroup.Element(yt + "duration").Attribute("seconds")),
                        Likes = (int)item.Element(yt + "rating").Attribute("numLikes"),
                        ViewCount = (int)item.Element(yt + "statistics").Attribute("viewCount"),
                        Thumbnail = new Uri(mediaGroup.Elements(media + "thumbnail").FirstOrDefault(o => o.Attribute(yt + "name").Value == "mqdefault").Attribute("url").Value)
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
            Index += MaxResults;
            if (Index >= TotalResults)
            {
                Index -= MaxResults;
                return;
            }
            CurrentPage = Index / MaxResults + 1;
            await GetDataForList();
        }
        public async void PreviousClick(object sender, EventArgs e)
        {
            Index -= MaxResults;
            if (Index < 1)
            {
                Index += MaxResults;
                return;
            }
            CurrentPage = Index / MaxResults + 1;
            await GetDataForList();
        }

        public async void SearchClick(object sender, EventArgs e)
        {
            var input = new InputPrompt();
            input.Completed += input_Completed;
            input.Title = "Search";
            input.Value = HttpUtility.UrlDecode(Query);
            input.Show();
        }

        private async void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            Index = 1;
            CurrentPage = Index / MaxResults + 1;
            Query = HttpUtility.UrlEncode(e.Result);
            await GetDataForList();
        }

        public async void PublishedClick(object sender, EventArgs e)
        {
            OrderBy = OrderByType.published.ToString();
            await GetDataForList();
        }

        public async void RatingClick(object sender, EventArgs e)
        {
            OrderBy = OrderByType.rating.ToString();
            await GetDataForList();
        }

        public async void ViewCountClick(object sender, EventArgs e)
        {
            OrderBy = OrderByType.viewCount.ToString();
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
            var emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Feedback of Larva";
            emailComposeTask.Body = "";
            emailComposeTask.To = "joy.entertainment@outlook.com";

            emailComposeTask.Show();
        }
    }
}