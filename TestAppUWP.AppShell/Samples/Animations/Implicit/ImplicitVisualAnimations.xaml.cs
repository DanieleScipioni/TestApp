using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace TestAppUWP.AppShell.Samples.Animations.Implicit
{
    public sealed partial class ImplicitVisualAnimations
    {
        public ImplicitVisualAnimations()
        {
            InitializeComponent();
            Visual visual = ElementCompositionPreview.GetElementVisual(Rectangle);
            visual.CenterPoint = new Vector3((float)Rectangle.ActualWidth / 2, (float)Rectangle.ActualHeight / 2, 0);
        }

        private void Vector3KeyFrameAnimationn_OnClick(object sender, RoutedEventArgs e)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(Rectangle);

            Compositor compositor = visual.Compositor;

            Vector3KeyFrameAnimation animation = Vector3KeyFrameAnimation(compositor);
            visual.StartAnimation(nameof(Visual.Scale), animation);
        }

        private void SpringVector3NaturalMotionAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(Rectangle);

            Compositor compositor = visual.Compositor;

            SpringVector3NaturalMotionAnimation animation = SpringVector3NaturalMotionAnimation(compositor);

            visual.StartAnimation(nameof(Visual.Scale), animation);
        }

        private void AddImplicitAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(Rectangle);
            SpringVector3NaturalMotionAnimation animation = SpringVector3NaturalMotionAnimation(visual.Compositor);
            animation.Target = nameof(Visual.Scale);
            ElementCompositionPreview.SetImplicitShowAnimation(Rectangle, animation);
            ElementCompositionPreview.SetImplicitHideAnimation(Rectangle, animation);
        }

        private void RemoveImplicitAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            ElementCompositionPreview.SetImplicitShowAnimation(Rectangle, null);
            ElementCompositionPreview.SetImplicitHideAnimation(Rectangle, null);
        }

        private void VisibilityAnimation_OnClick(object sender, RoutedEventArgs e)
        {
            Rectangle.Visibility = Rectangle.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private static Vector3KeyFrameAnimation Vector3KeyFrameAnimation(Compositor compositor)
        {
            Vector3KeyFrameAnimation animation = compositor.CreateVector3KeyFrameAnimation();
            animation.IterationBehavior = AnimationIterationBehavior.Count;
            animation.IterationCount = 1;
            animation.Duration = TimeSpan.FromSeconds(1);
            animation.InsertKeyFrame(0, new Vector3(0, 0, 0));
            animation.InsertKeyFrame(1, Vector3.One);
            return animation;
        }

        Vector3KeyFrameAnimation Vector3KeyFrameExpressionAnimation(Compositor compositor)
        {
            Vector3KeyFrameAnimation animation = compositor.CreateVector3KeyFrameAnimation();
            animation.InsertExpressionKeyFrame(0f, "this.StartingValue");
            animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            animation.Duration = TimeSpan.FromSeconds(1);
            return animation;
        }

        private static SpringVector3NaturalMotionAnimation SpringVector3NaturalMotionAnimation(Compositor compositor)
        {
            SpringVector3NaturalMotionAnimation animation = compositor.CreateSpringVector3Animation();
            animation.Period = TimeSpan.FromMilliseconds(60);
            animation.DampingRatio = 0.6f;
            animation.InitialValue = Vector3.Zero;
            animation.FinalValue = Vector3.One;
            return animation;
        }
    }
}
