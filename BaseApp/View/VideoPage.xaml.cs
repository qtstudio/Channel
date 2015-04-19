using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GoogleAds;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.NetworkInformation;
using Microsoft.PlayerFramework;
using MyToolkit.Multimedia;

namespace BaseApp.View
{
    public partial class VideoPage : PhoneApplicationPage
    {
        private InterstitialAd interstitialAd;
        public VideoPage()
        {
            InitializeComponent();
            //var adView = new AdView
            //{
            //    Format = AdFormats.SmartBanner,
            //    AdUnitID = "ca-app-pub-8337687806423142/6801746319"
            //};
            //adView.Margin = new Thickness(0, 0, 0, 405);
            //adView.ReceivedAd += OnAdReceived;
            //adView.FailedToReceiveAd += OnFailedToReceiveAd;
            //LayoutRoot.Children.Add(adView);
            //var adRequest = new AdRequest();
            //adView.LoadAd(adRequest);
            //player.PlayerStateChanged += PlayerStateChanged;
        }

        private void player_MediaClosed(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("closed");
        }

        private void PlayPauseVisibleChanged(object sender, EventArgs e)
        {
            MessageBox.Show("test");
        }

        private void PlayerStateChanged(object sender, RoutedPropertyChangedEventArgs<PlayerState> e)
        {
            if (e.NewValue == PlayerState.Ending)
            {
                interstitialAd = new InterstitialAd("ca-app-pub-8337687806423142/2694447517");
                var adRequest = new AdRequest();
                
                // Enable test ads.
                adRequest.ForceTesting = true;

                interstitialAd.ReceivedAd += OnAdReceivedFull;
                interstitialAd.LoadAd(adRequest);
            }
        }
        private void OnAdReceivedFull(object sender, AdEventArgs e)
        {
            interstitialAd.ShowAd();
        }
        private void OnAdReceived(object sender, AdEventArgs e)
        {
            //MessageBox.Show("Received ad successfully");
        }

        private void OnFailedToReceiveAd(object sender, AdErrorEventArgs errorCode)
        {
            //MessageBox.Show("Failed to receive ad with error " + errorCode.ErrorCode);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    string videoId = String.Empty;
                    if (NavigationContext.QueryString.TryGetValue("videoId", out videoId))
                    {
                        //Get The Video Uri and set it as a player source
                        var url = await YouTube.GetVideoUriAsync(videoId, YouTubeQuality.Quality480P);
                        player.Source = url.Uri;
                    }
                }
                else
                {
                    MessageBox.Show("You're not connected to Internet!");
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            base.OnNavigatedTo(e);
        }

    }
}