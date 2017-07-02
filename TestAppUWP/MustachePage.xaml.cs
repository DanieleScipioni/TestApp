using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace TestAppUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a MainFrame.
    /// </summary>
    public sealed partial class MustachePage
    {
        public MustachePage()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Target.Text = string.Empty;

            var data = new
            {
                Name = "Daniele",
                Value = 10000,
                TaxedValue = 5000,
                Currency = "euros",
                InCa = true
            };

            await Task.Delay(500);
            string template = Mustache.Template.Compile(Source.Text).Render(data);

            Target.Text = template;
        }
    }
}
