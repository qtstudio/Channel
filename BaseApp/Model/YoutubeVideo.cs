using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace BaseApp.Model
{
    public class YoutubeVideo : INotifyPropertyChanged
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
        public float Rating { get; set; }
        public TimeSpan Duration { get; set; }

        private WriteableBitmap _blurBgSource;
        public WriteableBitmap BlurBgSource
        {
            get { return _blurBgSource; }

            set
            {
                // Only update value if it changed
                if (value == _blurBgSource) return;
                _blurBgSource = value;

                // Call NotifyPropertyChanged when the property is updated
                NotifyPropertyChanged("BlurBgSource");
            }
        }

        // Declare the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }  
    }

    public enum OrderByType
    {
        published,
        rating,
        viewCount
    }
}
