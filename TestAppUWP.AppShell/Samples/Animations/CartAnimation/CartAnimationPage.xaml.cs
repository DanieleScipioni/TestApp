using System;
using System.Collections.Generic;
using TestAppUWP.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace TestAppUWP.Samples.Animations.CartAnimation
{
    public partial class CartAnimationPage
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        private FrameworkElement _animationTarget;
        private AddToCartAnimation _addToCartAnimation;
        private string _selectedAnimation;
        private readonly Brush _notHighligthedForeground;

        public CartAnimationPage()
        {
            _selectedAnimation = "1";
            DataContext = _cartAnimationViewModel = new CartAnimationViewModel();
            InitializeComponent();
            AppBarButton appBarButton = (AppBarButton) ((CommandBar)((Grid)Content).Children[0]).PrimaryCommands[0];
            _notHighligthedForeground = appBarButton.Foreground;
            appBarButton.Foreground = new SolidColorBrush(Colors.Red);
            Loaded += (sender, args) =>
            {
                _addToCartAnimation = new AddToCartAnimation(this);
                _animationTarget = CartPlaceholder;
            };
            Unloaded += (sender, args) =>
            {
                _addToCartAnimation.Dispose();
                
            };
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement)) return;

            DependencyObject dependencyObject = VisualTreeHelper.GetParent(frameworkElement);
            var image = (ContentPresenter)VisualTreeHelper.GetChild(dependencyObject, 0);

            switch (_selectedAnimation)
            {
                case "1":
                    await _addToCartAnimation.StartAnimation(image, _animationTarget);
                    break;
                case "2":
                    await _addToCartAnimation.StartAnimation2(image, _animationTarget);
                    break;
            }
            var stringItem = (StringItem) frameworkElement.DataContext;
            stringItem.Add.Execute(null);
        }

        private void AppBarButton_AnimationId_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!(sender is AppBarButton appBarButton)) return;
            var appBar = (StackPanel) VisualTreeHelper.GetParent(appBarButton);
            ((AppBarButton)appBar.Children[0]).Foreground = _notHighligthedForeground;
            ((AppBarButton)appBar.Children[1]).Foreground = _notHighligthedForeground;
            appBarButton.Foreground = new SolidColorBrush(Colors.Red);
            _selectedAnimation = appBarButton.Tag as string;
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
            _cartAnimationViewModel.Add();
        }

    }

    public class CartAnimationViewModel : BindableBase
    {
        private string _count;
        public List<StringItem> ItemSource { get; }

        public string Count
        {
            get => _count;
            private set => SetProperty(ref _count, value);
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

        public void Add()
        {
            if (int.TryParse(Count, out int count))
            {
                Count = (count + 1).ToString();
            }
        }
    }
}
