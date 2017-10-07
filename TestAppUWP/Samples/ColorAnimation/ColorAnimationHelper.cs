using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;

namespace TestAppUWP.Samples.ColorAnimation
{
    public static class ColorAnimationHelper
    {
        public static ColorAnimationUsingKeyFrames BlinkAnimation(Color startColor)
        {
            var toRed = new DiscreteColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                Value = Colors.Red
            };
            var keepRed = new DiscreteColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
                Value = Colors.Red
            };
            var back = new LinearColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1500)),
                Value = startColor
            };

            var colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames();
            ColorKeyFrameCollection colorKeyFrameCollection = colorAnimationUsingKeyFrames.KeyFrames;
            colorKeyFrameCollection.Add(toRed);
            colorKeyFrameCollection.Add(keepRed);
            colorKeyFrameCollection.Add(back);

            return colorAnimationUsingKeyFrames;
        }

        public static ColorAnimationUsingKeyFrames FreeAnimation(Color startColor)
        {
            // mutued from XAML example at https://docs.microsoft.com/en-us/uwp/api/Windows.UI.Xaml.Media.Animation.ColorAnimation

            var linearColorKeyFrame = new LinearColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(2000)),
                Value = Colors.Red
            };
            var discreteColorKeyFrame = new DiscreteColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(2500)),
                Value = Colors.Yellow
            };
            var splineColorKeyFrame = new SplineColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(4500)),
                Value = startColor,
                KeySpline = new KeySpline {ControlPoint1 = new Point(0.6, 0.0), ControlPoint2 = new Point(0.9, 0.0)}
            };

            var colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames();
            ColorKeyFrameCollection colorKeyFrameCollection = colorAnimationUsingKeyFrames.KeyFrames;
            colorKeyFrameCollection.Add(linearColorKeyFrame);
            colorKeyFrameCollection.Add(discreteColorKeyFrame);
            colorKeyFrameCollection.Add(splineColorKeyFrame);

            return colorAnimationUsingKeyFrames;
        }

        public static Timeline EaseAnimation(Color startColor)
        {
            // docs https://docs.microsoft.com/en-us/windows/uwp/graphics/key-frame-and-easing-function-animations#easing-functions

            var toRed = new EasingColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
                Value = Colors.Red,
                EasingFunction = new BounceEase {Bounces = 1, EasingMode = EasingMode.EaseIn}
            };
            var back = new EasingColorKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(2000)),
                Value = startColor,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseIn}
            };

            var colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames();
            ColorKeyFrameCollection colorKeyFrameCollection = colorAnimationUsingKeyFrames.KeyFrames;
            colorKeyFrameCollection.Add(toRed);
            colorKeyFrameCollection.Add(back);

            return colorAnimationUsingKeyFrames;
        }
    }
}
