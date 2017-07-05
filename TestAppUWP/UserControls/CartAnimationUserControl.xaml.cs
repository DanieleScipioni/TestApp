using System;
using MustacheDemo.Core;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null) return;

            CubicBezierEasingFunction cubicBezierEasingFunction = _compositor.CreateCubicBezierEasingFunction(new Vector2(0.5f, 0f), new Vector2(1f, 0.5f));

            Point point = frameworkElement.TransformToVisual(this).TransformPoint(new Point(0,0));
            Point targetPoint = CartPlaceholder.TransformToVisual(this).TransformPoint(new Point(CartPlaceholder.ActualWidth / 2, CartPlaceholder.ActualHeight / 2));

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateColorBrush(Color.FromArgb(128, 0, 139, 139));
            spriteVisual.Size = new Vector2((float) frameworkElement.Width, (float)frameworkElement.Height);
            spriteVisual.Offset = new Vector3((float) point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtTop(spriteVisual);

            ScalarKeyFrameAnimation yOffsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            yOffsetAnimation.InsertKeyFrame(1f, (float)targetPoint.Y, cubicBezierEasingFunction);
            SetAnimationDefautls(yOffsetAnimation);

            ScalarKeyFrameAnimation xOffsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            xOffsetAnimation.InsertKeyFrame(1f, (float)targetPoint.X, cubicBezierEasingFunction);
            SetAnimationDefautls(xOffsetAnimation);

            ScalarKeyFrameAnimation widthAnimation = _compositor.CreateScalarKeyFrameAnimation();
            widthAnimation.InsertKeyFrame(1f, 0f, cubicBezierEasingFunction);
            SetAnimationDefautls(widthAnimation);

            ScalarKeyFrameAnimation heigthAnimation = _compositor.CreateScalarKeyFrameAnimation();
            heigthAnimation.InsertKeyFrame(1f, 0f, cubicBezierEasingFunction);
            SetAnimationDefautls(heigthAnimation);

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            
            spriteVisual.StartAnimation("Offset.Y", yOffsetAnimation);
            spriteVisual.StartAnimation("Offset.X", xOffsetAnimation);
            spriteVisual.StartAnimation("Size.X", widthAnimation);
            spriteVisual.StartAnimation("Size.Y", heigthAnimation);
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

        private static void SetAnimationDefautls(ScalarKeyFrameAnimation animation)
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
