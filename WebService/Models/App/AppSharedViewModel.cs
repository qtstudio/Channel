using System.ComponentModel;

namespace WebService.Models.App
{
    public class AppSharedViewModel : INotifyPropertyChanged
    {
        public string ShortDesc { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }

            set
            {
                // Only update value if it changed
                if (value == _name) return;
                _name = value;

                // Call NotifyPropertyChanged when the property is updated
                NotifyPropertyChanged("Name");
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
}
