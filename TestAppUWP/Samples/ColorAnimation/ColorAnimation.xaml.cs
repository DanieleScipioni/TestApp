using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using TestAppUWP.Samples.ColorAnimation;

namespace TestAppUWP.Samples.ColorAnimation
{
    public sealed partial class ColorAnimation
    {
        private ColorAnimationViewModel _vm;
        private readonly Storyboard _storyboard;

        public ColorAnimation()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is ColorAnimationViewModel implicitAnimationViewModel &&
                    _vm != implicitAnimationViewModel)
                {
                    _vm = implicitAnimationViewModel;
                }
            };

            _storyboard = new Storyboard();

            var blinkColorBursh = (SolidColorBrush) TextBlockBlink.Foreground;
            ColorAnimationUsingKeyFrames blinkAnimation = ColorAnimationHelper.BlinkAnimation(blinkColorBursh.Color);
            AnimateTextBlockForeground(TextBlockBlink, blinkAnimation);

            var freeColorBrush = (SolidColorBrush) TextBlockFree.Foreground;
            ColorAnimationUsingKeyFrames freeAnimation = ColorAnimationHelper.FreeAnimation(freeColorBrush.Color);
            AnimateTextBlockForeground(TextBlockFree, freeAnimation);

            var easeColorBrush = (SolidColorBrush) TextBlockEase.Foreground;
            Timeline easeAnimation = ColorAnimationHelper.EaseAnimation(easeColorBrush.Color);
            AnimateTextBlockForeground(TextBlockEase, easeAnimation);

            _storyboard.Children.Add(blinkAnimation);
            _storyboard.Children.Add(freeAnimation);
            _storyboard.Children.Add(easeAnimation);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _storyboard.Begin();
        }

        public static void AnimateTextBlockForeground(TextBlock textBlock, Timeline timeline)
        {
            Storyboard.SetTarget(timeline, textBlock);
            Storyboard.SetTargetProperty(timeline, "TextBlock.Foreground.Color");
        }
    }
}
