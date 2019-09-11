using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

namespace TestAppUWP.Samples.Animations.DropShadow
{
    public sealed partial class DropShadowPage
    {
        public DropShadowPage()
        {
            InitializeComponent();

            Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            Windows.UI.Composition.DropShadow dropShadow = compositor.CreateDropShadow();
            dropShadow.BlurRadius = 10f;
            dropShadow.Color = Colors.Black;
            
            SpriteVisual spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Shadow = dropShadow;

            ElementCompositionPreview.SetElementChildVisual(Button, spriteVisual);

            Visual buttonVisual = ElementCompositionPreview.GetElementVisual(Button);
            Visual borderVisual = ElementCompositionPreview.GetElementVisual(Border);

            Button.SizeChanged += (sender, args) =>
            {
                spriteVisual.Size = new Vector2((float) Button.ActualWidth, (float) Button.ActualWidth);
            };
        }
    }
}
