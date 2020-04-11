using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.Samples.PathButton
{
    public class StretchPathIcon : Windows.UI.Xaml.Controls.PathIcon
    {
        private Path _path;

        public StretchPathIcon()
        {
            Loaded += PathIconStr_Loaded;
        }

        private void PathIconStr_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = (Grid)VisualTreeHelper.GetChild(this, 0);
            _path = (Path) grid.Children[0];
            _path.Stretch = Stretch;
        }

        #region DependencyProperties

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            nameof(Stretch), typeof(Stretch), typeof(StretchPathIcon), new PropertyMetadata(default(Stretch), StretchPropertyChangedCallback));


        private static void StretchPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stretchPathIcon = (StretchPathIcon) d;
            Path path = stretchPathIcon._path;
            if (path != null) path.Stretch = (Stretch) e.NewValue;
        }

        public Stretch Stretch
        {
            get => (Stretch) GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        #endregion
    }
}
