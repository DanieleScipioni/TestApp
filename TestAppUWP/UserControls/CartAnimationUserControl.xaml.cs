using System;
using MustacheDemo.Core;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using TestAppUWP.Core;

namespace TestAppUWP.UserControls
{
    public sealed partial class CartAnimationUserControl
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        private Compositor _compositor;
        private ContainerVisual _containerVisual;

        public CartAnimationUserControl()
        {
            DataContext = _cartAnimationViewModel = new CartAnimationViewModel();
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                Visual visual = ElementCompositionPreview.GetElementVisual(this);
                _compositor = visual.Compositor;
                _containerVisual = _compositor.CreateContainerVisual();
                ElementCompositionPreview.SetElementChildVisual(this, _containerVisual);
            };
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            CubicBezierEasingFunction cubicBezierEasingFunction = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.5f, 0f), new Vector2(1f, 0.5f));

            Point point = frameworkElement.TransformToVisual(this).TransformPoint(new Point(0,0));
            Point targetPoint = CartPlaceholder.TransformToVisual(this).TransformPoint(new Point(CartPlaceholder.ActualWidth / 2, CartPlaceholder.ActualHeight / 2));

            var bitmapImage = new BitmapImage();
            using (InMemoryRandomAccessStream stream = await ((UIElement)sender).GetBitmapImageStream())
                bitmapImage.SetSource(stream);
            var image = new Image
            {
                Height = frameworkElement.Height,
                Width = frameworkElement.Width,
                Source = bitmapImage,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            var imageBrush = new ImageBrush { ImageSource = bitmapImage };

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateColorBrush(Color.FromArgb(128, 0, 139, 139));
            spriteVisual.Size = new Vector2((float) frameworkElement.Width, (float)frameworkElement.Height);
            spriteVisual.Offset = new Vector3((float) point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtTop(spriteVisual);

            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, new Vector3((float)targetPoint.X, (float)targetPoint.Y, 0), cubicBezierEasingFunction);
            SetAnimationDefautls(offsetAnimation);

            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();
            sizeAnimation.InsertKeyFrame(1f, new Vector2(0f, 0f), cubicBezierEasingFunction);
            SetAnimationDefautls(sizeAnimation);

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            
            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();

            void BatchCompleted(object source, CompositionBatchCompletedEventArgs args)
            {
                _containerVisual.Children.Remove(spriteVisual);
                spriteVisual.Dispose();
                myScopedBatch.Completed -= BatchCompleted;
                myScopedBatch.Dispose();
            }
            myScopedBatch.Completed += BatchCompleted;

        }

        private static void SetAnimationDefautls(KeyFrameAnimation animation)
        {
            TimeSpan animationDuration = TimeSpan.FromMilliseconds(500);
            animation.Duration = animationDuration;
            animation.IterationBehavior = AnimationIterationBehavior.Count;
            animation.IterationCount = 1;
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
        }
    }

    public class StringItem : BindableBase
    {
        public string V { get; }
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        public DelegateCommand Add { get; }

        public StringItem(string v, CartAnimationViewModel cartAnimationViewModel)
        {
            V = v;
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
            ItemSource = new List<StringItem>
            {
                new StringItem("1", this),
                new StringItem("2", this),
                new StringItem("3", this),
                new StringItem("4", this),
                new StringItem("5", this),
                new StringItem("6", this),
                new StringItem("7", this),
                new StringItem("8", this)
            };

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
