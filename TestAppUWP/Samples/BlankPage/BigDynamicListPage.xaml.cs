using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.Samples.BlankPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a MainFrame.
    /// </summary>
    public sealed partial class BigDynamicListPage
    {
        private PageViewModel _pageViewModel;

        public BigDynamicListPage()
        {
            InitializeComponent();
            ((ThisPageConverter) Resources["ThisPageConverter"]).BigDynamicListPage = this;
            Loaded += (sender, args) =>
            {
                DataContext = new PageViewModel();
            };
            DataContextChanged += (sender, args) =>
            {
                if (_pageViewModel == args.NewValue) return;
                _pageViewModel = (PageViewModel)args.NewValue;
                Bindings.Update();
            };
        }

        public async Task DoSomething(string textBlockText)
        {
            var contentDialog = new ContentDialog {Content = $"Ciao {textBlockText}", Title = "Title", PrimaryButtonText = "Primary"};
            await contentDialog.ShowAsync();
        }
    }

    internal class Pippo 
    {
        public int Intero;
    }

    class PageViewModel : BindableBase
    {
        public PageViewModel()
        {
            var random = new Random(DateTime.UtcNow.Millisecond);
            var ints = new List<PippoCollection>();
            for (var i = 0; i < 100000; i++)
            {
                var a = new PippoCollection();
                for (var j = 0; j < 30; j++)
                {
                    a.Add(new Pippo { Intero = random.Next() });
                }
                ints.Add(a);
            }
            Ints = ints;

        }

        private List<PippoCollection> _ints;
        public List<PippoCollection> Ints
        {
            get => _ints;
            set => SetProperty(ref _ints, value);
        }
    }

    internal class PippoCollection : List<Pippo> {}

    internal class ThisPageConverter : IValueConverter
    {
        public BigDynamicListPage BigDynamicListPage { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return BigDynamicListPage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
