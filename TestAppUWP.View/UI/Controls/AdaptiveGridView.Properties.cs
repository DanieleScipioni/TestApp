using Windows.Foundation;
using Windows.UI.Xaml;

namespace TestAppUWP.View.UI.Controls
{
    public partial class AdaptiveGridView
    {
        public static readonly DependencyProperty DesiredMeasureProperty =
            DependencyProperty.Register(nameof(DesiredMeasure), typeof(double), typeof(AdaptiveGridView),
                new PropertyMetadata(double.NaN, DesiredMeasureChanged));

        private static void DesiredMeasureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (AdaptiveGridView) d;
            self.RecalculateLayout(new Size(self.ActualWidth, self.ActualHeight));
        }

        public double DesiredMeasure
        {
            get => (double)GetValue(DesiredMeasureProperty);
            set => SetValue(DesiredMeasureProperty, value);
        }
    }
}
