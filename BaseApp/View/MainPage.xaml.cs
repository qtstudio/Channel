﻿using System;
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
using BaseApp.Model;
using BaseApp.Utilities;

namespace BaseApp.View
{
    public partial class MainPage
    {
        private readonly Configuration _configuration  = ((App)Application.Current).Configuration; 
        private readonly DetectorLongList _detectorLongList;

        private int _maxResult = 10;
        private string _orderBy = "published";
        private int _index = 1;
        private string _query = "";

        public MainPage()
        {
            InitializeComponent();
            FeedbackOverlay.VisibilityChanged += FeedbackOverlay_VisibilityChanged;
            
            _detectorLongList = new DetectorLongList();
            _detectorLongList.Compression += GetMoreVideos;
            _detectorLongList.Bind(ChannelVideos);
        }
        public InterfaceViewModel InterfaceViewModel
        {
            get { return _configuration.InterfaceViewModel; }
        }

        public ColorSchemeViewModel ColorSchemeViewModel
        {
            get { return InterfaceViewModel.ColorSchemeViewModel; }
        }
        public List<ChannelViewModel> ChannelViewModel
        {
            get { return _configuration.ChannelViewModel; }
        }

        private async void GetMoreVideos(object sender, CompressionEventArgs compressionEventArgs)
        {
            if (compressionEventArgs.Type == CompressionType.Bottom)
            {
                _index += _maxResult;
                await GetDataForList("Loading more videos...");
            }
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

                    await GetDataForList("Loading videos...");
                    Indicator.Text = "";
                    Indicator.IsIndeterminate = false;
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

        private async Task GetDataForList(string contentLoading)
        {
            Indicator.Text = contentLoading;
            Indicator.IsIndeterminate = true;

            var channelInfo = ChannelViewModel[0];

            var channelVideos =
                await
                    GetYoutubeChannel("http://gdata.youtube.com/feeds/api/users/" + channelInfo.ChannelId +
                                      "/uploads?alt=" + channelInfo.TypeData + "&v=2&orderby=" + _orderBy + "&start-index=" + _index +
                                      "&max-results=" + _maxResult + (string.IsNullOrEmpty(_query) ? "" : ("&q=" + _query)));
            if (ChannelVideos.ItemsSource == null)
                ChannelVideos.ItemsSource = channelVideos;
            else
            {
                _detectorLongList.Unbind();
                foreach (var item in channelVideos)
                {
                    ChannelVideos.ItemsSource.Add(item);
                }
                _detectorLongList.Bind(ChannelVideos);
            }
            
        }

        private async Task<List<YoutubeVideo>> GetYoutubeChannel(string url)
        {
            try
            {
                var client = new HttpClient();
                var feedXml = await client.GetStringAsync(new Uri(url));

                //var atomns = XNamespace.Get("http://www.w3.org/2005/Atom");
                var yt = XNamespace.Get("http://gdata.youtube.com/schemas/2007");
                //var openSearch = XNamespace.Get("http://a9.com/-/spec/opensearch/1.1/");
                var media = XNamespace.Get("http://search.yahoo.com/mrss/");
                var gd = XNamespace.Get("http://schemas.google.com/g/2005");

                var xml = XElement.Parse(feedXml);
                var channel = xml.Element("channel");
                var items = channel.Elements("item");
                var videosList = new List<YoutubeVideo>();
                //TotalResults = int.Parse(channel.Element(openSearch + "totalResults").Value);
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

        //public async void NextClick(object sender, EventArgs e)
        //{
        //    _configuration.Index += _configuration.MaxResult;
        //    if (_configuration.Index >= TotalResults)
        //    {
        //        _configuration.Index -= _configuration.MaxResult;
        //        return;
        //    }
        //    CurrentPage = _configuration.Index / _configuration.MaxResult + 1;
        //    await GetDataForList();
        //}
        //public async void PreviousClick(object sender, EventArgs e)
        //{
        //    _configuration.Index -= _configuration.MaxResult;
        //    if (_configuration.Index < 1)
        //    {
        //        _configuration.Index += _configuration.MaxResult;
        //        return;
        //    }
        //    CurrentPage = _configuration.Index / _configuration.MaxResult + 1;
        //    await GetDataForList();
        //}

        public async void SearchClick(object sender, EventArgs e)
        {
            var input = new InputPrompt();
            input.Completed += input_Completed;
            input.Title = "Search";
            input.Value = HttpUtility.UrlDecode(_query);
            input.Show();
        }

        private async void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            //_configuration.Index = 1;
            //CurrentPage = _configuration.Index / _configuration.MaxResult + 1;
            _query = HttpUtility.UrlEncode(e.Result);
            await GetDataForList("Searching...");
        }

        public async void PublishedClick(object sender, EventArgs e)
        {
            _orderBy = OrderByType.published.ToString();
            await GetDataForList("Loading videos...");
        }

        public async void RatingClick(object sender, EventArgs e)
        {
            _orderBy = OrderByType.rating.ToString();
            await GetDataForList("Loading videos...");
        }

        public async void ViewCountClick(object sender, EventArgs e)
        {
            _orderBy = OrderByType.viewCount.ToString();
            await GetDataForList("Loading videos...");
        }

        public void RateAndReviewClick(object sender, EventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        public async void ReloadClick(object sender, EventArgs e)
        {
            await GetDataForList("Loading videos...");
        }

        public void FeedbackClick(object sender, EventArgs e)
        {
            var emailComposeTask = new EmailComposeTask
            {
                Subject = "Feedback of Larva",
                Body = "",
                To = "joy.entertainment@outlook.com"
            };

            emailComposeTask.Show();
        }
    }
}