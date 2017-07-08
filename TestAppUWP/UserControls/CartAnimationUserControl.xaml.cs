using MustacheDemo.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;

namespace TestAppUWP.UserControls
{
    public sealed partial class CartAnimationUserControl
    {
        private readonly CartAnimationViewModel _cartAnimationViewModel;

        private Compositor _compositor;
        private ContainerVisual _containerVisual;
        private CanvasDevice _canvasDevice;
        private CompositionGraphicsDevice _graphicsDevice;

        public CartAnimationUserControl()
        {
            DataContext = _cartAnimationViewModel = new CartAnimationViewModel();
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                _containerVisual = _compositor.CreateContainerVisual();
                ElementCompositionPreview.SetElementChildVisual(this, _containerVisual);
                _canvasDevice = new CanvasDevice();
                _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _canvasDevice);
            };
            Unloaded += (sender, args) =>
            {
                _compositor.Dispose();
                _containerVisual.Dispose();
                _canvasDevice.Dispose();
                _graphicsDevice.Dispose();
            };
        }

        public async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            Point point = frameworkElement.TransformToVisual(this).TransformPoint(new Point(0,0));
            Point targetPoint = CartPlaceholder.TransformToVisual(this).TransformPoint(new Point(CartPlaceholder.ActualWidth / 2, CartPlaceholder.ActualHeight / 2));

            CompositionDrawingSurface surface;
            using (CanvasBitmap canvasBitmap = await CanvasBitmap.LoadAsync(_canvasDevice, new Uri("ms-appx:///Assets/Photos/pic00.jpg")))
            {
                surface = _graphicsDevice.CreateDrawingSurface(new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight),
                    DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Ignore);
                using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(surface))
                {
                    session.Clear(Color.FromArgb(0, 0, 0, 0));
                    session.DrawImage(canvasBitmap, new Rect(0, 0, surface.Size.Width, surface.Size.Height),
                        new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height));
                }
            }

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateSurfaceBrush(surface);
            spriteVisual.Size = new Vector2((float) frameworkElement.Width, (float)frameworkElement.Height);
            spriteVisual.Offset = new Vector3((float) point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtTop(spriteVisual);

            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, new Vector3((float)targetPoint.X, (float)targetPoint.Y, 0));
            SetAnimationDefautls(offsetAnimation);

            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();
            sizeAnimation.InsertKeyFrame(1f, new Vector2(0f, 0f));
            SetAnimationDefautls(sizeAnimation);

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();

            void BatchCompleted(object source, CompositionBatchCompletedEventArgs args)
            {
                spriteVisual.Dispose();
                myScopedBatch.Completed -= BatchCompleted;
                myScopedBatch.Dispose();
            }
            myScopedBatch.Completed += BatchCompleted;

        }

        private static void SetAnimationDefautls(KeyFrameAnimation animation)
        {
            TimeSpan animationDuration = TimeSpan.FromMilliseconds(1500);
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
