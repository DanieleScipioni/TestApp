using Windows.UI.Xaml.Media;

namespace TestAppUWP.PathIcon
{
    public class ExtendedPathIcon : Windows.UI.Xaml.Controls.PathIcon
    {
        public ExtendedPathIcon()
        {
            Loaded += ExtendedPathIcon_Loaded;
        }

        private void ExtendedPathIcon_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Geometry geometry = this.Data;
        }

        public new Geometry Data
        {
            get { return base.Data; }
            set { base.Data = value; }
        }
    }
}
