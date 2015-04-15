using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace YoutubeVideoSampleWP80.Utilities
{
    public class DetectorLongList
    {
        private LongListSelector _listbox;
        private ViewportControl _viewport = null;
        private bool _viewportChanged = false;
        private bool _isMoving = false;
        private double _manipulationStart = 0;
        private double _manipulationEnd = 0;
        public bool Bound { get; private set; }

        public void Bind(LongListSelector l)
        {
            Bound = true;
            this._listbox = l;
            _listbox.ManipulationStateChanged += listbox_ManipulationStateChanged;
            _listbox.MouseMove += listbox_MouseMove;
            _listbox.ItemRealized += OnViewportChanged;
            _listbox.ItemUnrealized += OnViewportChanged;
            _listbox.Loaded += listbox_Loaded;
        }
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }
                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
        public void Unbind()
        {
            Bound = false;
            if (_listbox == null)
                return;
            _listbox.ManipulationStateChanged -= listbox_ManipulationStateChanged;
            _listbox.MouseMove -= listbox_MouseMove;
            _listbox.ItemRealized -= OnViewportChanged;
            _listbox.ItemUnrealized -= OnViewportChanged;
        }

        private void listbox_Loaded(object sender, RoutedEventArgs e)
        {
            this._viewport = FindVisualChild<ViewportControl>(_listbox);
        }
        private void OnViewportChanged(object sender, Microsoft.Phone.Controls.ItemRealizationEventArgs e)
        {
            _viewportChanged = true;
        }
        private void listbox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pos = e.GetPosition(null);
            if (!_isMoving)
                _manipulationStart = pos.Y;
            else
                _manipulationEnd = pos.Y;
            _isMoving = true;
        }
        private void listbox_ManipulationStateChanged(object sender, EventArgs e)
        {
            if (_listbox.ManipulationState == ManipulationState.Idle)
            {
                _isMoving = false;
                _viewportChanged = false;
            }
            else if (_listbox.ManipulationState == ManipulationState.Manipulating)
            {
                _viewportChanged = false;
            }
            else if (_listbox.ManipulationState == ManipulationState.Animating)
            {
                var total = _manipulationStart - _manipulationEnd;
                if (!_viewportChanged && Compression != null)
                {
                    if (total < 0 && (_viewport.Viewport.Top == _viewport.Bounds.Top))
                        Compression(this, new CompressionEventArgs(CompressionType.Top)); 
                    else if (total > 0 && (_viewport.Bounds.Bottom - (_viewport.Viewport.Height + _viewport.Viewport.Bottom) < 0))
                        Compression(this, new CompressionEventArgs(CompressionType.Bottom));
                }
            }
        }
        public event OnCompression Compression;
    }

    public class CompressionEventArgs : EventArgs
    {
        public CompressionType Type { get; protected set; }
        public CompressionEventArgs(CompressionType type)
        {
            Type = type;
        }
    }

    public enum CompressionType { Top, Bottom, Left, Right };
    public delegate void OnCompression(object sender, CompressionEventArgs e);
}
