using System;
using System.Windows.Media.Imaging;

namespace YoutubeVideoSampleWP80.Model
{
    public class YoutubeVideo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime PubDate { get; set; }
        public Uri YoutubeLink { get; set; }
        public Uri VideoLink { get; set; }
        public Uri Thumbnail { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public int ViewCount { get; set; }
        public TimeSpan Duration { get; set; }
        public WriteableBitmap BlurBgSource { get; set; }
    }

    public enum OrderByType
    {
        published,
        rating,
        viewCount
    }
}
