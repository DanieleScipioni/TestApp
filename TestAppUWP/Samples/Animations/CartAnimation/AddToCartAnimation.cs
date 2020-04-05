using System;
using System.Numerics;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace TestAppUWP.Samples.Animations.CartAnimation
{
    public class AddToCartAnimation : IDisposable
    {
        private readonly FrameworkElement _rootElement;
        private readonly Compositor _compositor;
        private readonly ContainerVisual _containerVisual;

        public AddToCartAnimation(FrameworkElement rootElement)
        {
            _rootElement = rootElement;
            _compositor = ElementCompositionPreview.GetElementVisual(_rootElement).Compositor;
            _containerVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(_rootElement, _containerVisual);
        }

        public void Dispose()
        {
            _containerVisual.Dispose();
            //_compositor.Dispose();
        }

        public async Task StartAnimation(FrameworkElement sourceElement, FrameworkElement targetElement)
        {
            Point point = sourceElement.TransformToVisual(_rootElement).TransformPoint(new Point(0, 0));

            CompositionDrawingSurface surface = await CompositionDrawingSurfaceFacade1.GetCompositionDrawingSurface(sourceElement, _compositor);

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateSurfaceBrush(surface);
            spriteVisual.Size = new Vector2((float)surface.Size.Width, (float)surface.Size.Height);
            spriteVisual.Offset = new Vector3((float)point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtBottom(spriteVisual);

            Vector3 targetOffset = GetTargetOffset(targetElement);
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            SetAnimationDefautls(offsetAnimation);

            Vector2 targetSize = GetTargetSize(targetElement);
            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();
            SetAnimationDefautls(sizeAnimation);

            var newWidth = (float)(sourceElement.ActualWidth * 1.3);
            var newHeight = (float)(sourceElement.ActualHeight * 1.3);
            var newX = (float)(point.X - (newWidth - sourceElement.ActualWidth) / 2);
            var newY = (float)(point.Y - (newHeight - sourceElement.ActualHeight) / 2);

            const float normalizedProgressKey0 = 0.3f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector3(newX, newY, 0f));
            sizeAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector2(newWidth, newHeight));

            const float normalizedProgressKey1 = 1f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey1, targetOffset, _compositor.CreateLinearEasingFunction());
            sizeAnimation.InsertKeyFrame(normalizedProgressKey1, targetSize, _compositor.CreateLinearEasingFunction());

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            var batchCompletitionHandler = new BatchCompletitionAwaiter(myScopedBatch);

            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();
            await batchCompletitionHandler.Completed();

            myScopedBatch.Dispose();
            spriteVisual.Dispose();
            surface.Dispose();
            offsetAnimation.Dispose();
            sizeAnimation.Dispose();
        }

        // This animation has constant duration, speedy changes depending on the distance
        // between sourceElement and targetElement.
        public async Task StartAnimation2(FrameworkElement sourceElement, FrameworkElement targetElement)
        {
            Point point = sourceElement.TransformToVisual(_rootElement).TransformPoint(new Point(0, 0));

            CompositionDrawingSurface surface = await CompositionDrawingSurfaceFacade1.GetCompositionDrawingSurface(sourceElement, _compositor);

            SpriteVisual spriteVisual = _compositor.CreateSpriteVisual();
            spriteVisual.Brush = _compositor.CreateSurfaceBrush(surface);
            spriteVisual.Size = new Vector2((float)surface.Size.Width, (float)surface.Size.Height);
            spriteVisual.Offset = new Vector3((float)point.X, (float)point.Y, 0f);
            _containerVisual.Children.InsertAtBottom(spriteVisual);

            Vector3 targetOffset = GetTargetOffset(targetElement);
            Vector3KeyFrameAnimation offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();

            Vector2 targetSize = GetTargetSize(targetElement);
            Vector2KeyFrameAnimation sizeAnimation = _compositor.CreateVector2KeyFrameAnimation();

            var newWidth = (float)(sourceElement.ActualWidth * 1.3);
            var newHeight = (float)(sourceElement.ActualHeight * 1.3);
            var newX = (float)(point.X - (newWidth - sourceElement.ActualWidth) / 2);
            var newY = (float)(point.Y - (newHeight - sourceElement.ActualHeight) / 2);

            double sizeDurationInMs = 250;
            double distance = Math.Sqrt(Math.Pow(targetOffset.X - newX, 2) + Math.Pow(targetOffset.Y - newY, 2));
            double offsetDurationInMs = distance / 2; 
            double animationDurationInMs = sizeDurationInMs + offsetDurationInMs;

            sizeAnimation.Duration = TimeSpan.FromMilliseconds(animationDurationInMs);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(animationDurationInMs);

            SetAnimationDefautls(offsetAnimation);
            SetAnimationDefautls(sizeAnimation);

            var normalizedProgressKey0 = (float) (sizeDurationInMs / animationDurationInMs);
            offsetAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector3(newX, newY, 0f));
            sizeAnimation.InsertKeyFrame(normalizedProgressKey0, new Vector2(newWidth, newHeight));

            const float normalizedProgressKey1 = 1f;
            offsetAnimation.InsertKeyFrame(normalizedProgressKey1, targetOffset, _compositor.CreateLinearEasingFunction());
            sizeAnimation.InsertKeyFrame(normalizedProgressKey1, targetSize, _compositor.CreateLinearEasingFunction());

            CompositionScopedBatch myScopedBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var batchCompletitionAwaiter = new BatchCompletitionAwaiter(myScopedBatch);

            spriteVisual.StartAnimation("Offset", offsetAnimation);
            spriteVisual.StartAnimation("Size", sizeAnimation);
            myScopedBatch.End();
            await batchCompletitionAwaiter.Completed();

            myScopedBatch.Dispose();
            spriteVisual.Dispose();
            surface.Dispose();
            offsetAnimation.Dispose();
            sizeAnimation.Dispose();
        }

        private Vector3 GetTargetOffset(FrameworkElement frameworkElement)
        {
            Point targetPoint = frameworkElement.TransformToVisual(_rootElement)
                .TransformPoint(new Point(0, 0));
            return new Vector3((float)targetPoint.X, (float)targetPoint.Y, 0f);
        }

        private static Vector2 GetTargetSize(FrameworkElement frameworkElement)
        {
            return new Vector2((float)frameworkElement.ActualWidth, (float)frameworkElement.ActualHeight);
        }

        private static void SetAnimationDefautls(KeyFrameAnimation animation)
        {
            animation.IterationBehavior = AnimationIterationBehavior.Count;
            animation.IterationCount = 1;
        }
    }
}
