using Windows.UI.Xaml;

namespace TestAppUWP.AppShell.Samples.Test
{
    public sealed partial class RandomTestsUc
    {
        public RandomTestsUc()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(RandomTestsUc),
            new PropertyMetadata(default(string), (o, args) =>
            {
                var randomTestsUc = (RandomTestsUc) o;
                randomTestsUc.TextBlock.Text = (string) args.NewValue;
            }));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
