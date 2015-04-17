﻿using System.ComponentModel;
using Windows.UI;

namespace YoutubeVideoSampleWP80.Model
{
    public class ColorSchemeViewModel : INotifyPropertyChanged
    {
        private Color darkColor;

        public Color DarkColor
        {
            get { return darkColor; }
            set
            {
                // Only update value if it changed
                if (value == darkColor) return;
                darkColor = value;

                // Call NotifyPropertyChanged when the property is updated
                NotifyPropertyChanged("DarkColor");
                
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

        public Color LightColor { get; set; }
        public Color DarkTextColor { get; set; }
        public Color LightTextColor { get; set; }
    }
}