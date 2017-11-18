using System;
using System.Collections.Generic;
using TestAppUWP.Core;

namespace TestAppUWP.Samples.BlankPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a MainFrame.
    /// </summary>
    public sealed partial class MustachePage
    {
        private PageViewModel _pageViewModel;

        public MustachePage()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                DataContext = new PageViewModel();
            };
            DataContextChanged += (sender, args) =>
            {
                _pageViewModel = (PageViewModel) DataContext;
            };
        }
    }

    class Pippo
    {
        public int Intero;
    }

    class PageViewModel : BindableBase
    {
        public PageViewModel()
        {
            var random = new Random(DateTime.UtcNow.Millisecond);
            var ints = new List<Pippo[]>();
            for (var i = 0; i < 100000; i++)
            {
                var a = new Pippo[1000];
                for (var j = 0; j < 1000; j++)
                {
                    a[j] = new Pippo { Intero = random.Next() };
                }
                ints.Add(a);
            }
            Ints = ints;

        }

        private List<Pippo[]> _ints;
        public List<Pippo[]> Ints
        {
            get => _ints;
            set => SetProperty(ref _ints, value);
        }
    }
}
