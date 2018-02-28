using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using TestAppUWP.Core;
using TestAppUWP.UserControls.CartAnimation;

namespace TestAppUWP.Samples.CartAnimation
{
    public sealed partial class CartAnimationUserControl
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        private Viewbox _viewbox;
        private AddToCartAnimation _addToCartAnimation;

        public CartAnimationUserControl()
        {
            DataContext = _cartAnimationViewModel = new CartAnimationViewModel();
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                _addToCartAnimation = new AddToCartAnimation(this);
                _viewbox = null; //VisualTreeHelper.CartPlaceholder.FindDescendant<Viewbox>();
            };
            Unloaded += (sender, args) =>
            {
                _addToCartAnimation.Dispose();
                
            };
        }

        public async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            DependencyObject dependencyObject = VisualTreeHelper.GetParent(frameworkElement);
            var image = (ContentPresenter)VisualTreeHelper.GetChild(dependencyObject, 0);

            await _addToCartAnimation.StartAnimation2(image, _viewbox);
            var stringItem = (StringItem) frameworkElement.DataContext;
            stringItem.Add.Execute(null);
        }
    }

    public class StringItem : BindableBase
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;
        public string Text { get; }
        public Uri ImageUri { get; }

        public DelegateCommand Add { get; }

        public StringItem(string text, Uri imageUri, CartAnimationViewModel cartAnimationViewModel)
        {
            Text = text;
            ImageUri = imageUri;
            _cartAnimationViewModel = cartAnimationViewModel;
            Add = new DelegateCommand(AddExecute);
        }

        private void AddExecute(object parameter)
        {
            _cartAnimationViewModel.Add(this);
        }

    }

    public class CartAnimationViewModel : BindableBase
    {
        private string _count;
        public List<StringItem> ItemSource { get; }

        public string Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public CartAnimationViewModel()
        {
            _count = "0";

            ItemSource = new List<StringItem>();
            for (var i = 0; i < 46; i++)
            {
                string fileNumber = i.ToString("00");
                ItemSource.Add(new StringItem(fileNumber, new Uri($"ms-appx:///Assets/Photos/pic{fileNumber}.jpg"),
                    this));
            }
        }

        public void Add(StringItem stringItem)
        {
            if (int.TryParse(Count, out int count))
            {
                Count = (count + 1).ToString();
            }
        }
    }
}
