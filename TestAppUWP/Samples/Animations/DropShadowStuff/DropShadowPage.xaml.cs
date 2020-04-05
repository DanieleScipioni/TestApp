using System.Numerics;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

namespace TestAppUWP.Samples.Animations.DropShadowStuff
{
    public sealed partial class DropShadowPage
    {
        public DropShadowPage()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                await DropShadowMethod1(compositor);
            };
        }

        private async Task DropShadowMethod1(Compositor compositor)
        {
            // Compare cart animation code with
            // https://stackoverflow.com/questions/38361081/uwp-composition-apply-opacity-mask-to-top-30px-of-a-listview

            DropShadow dropShadow = compositor.CreateDropShadow();
            dropShadow.BlurRadius = 10f;
            dropShadow.Color = Colors.Black;
            dropShadow.Offset = new Vector3(10f, 10f, 0f);
            dropShadow.Mask = await CompositionDrawingSurfaceFacade2.GetCompositionSurfaceBrush(Button, compositor);
            //dropShadow.SourcePolicy = CompositionDropShadowSourcePolicy.InheritFromVisualContent;

            LayerVisual layerVisual = compositor.CreateLayerVisual();

            SpriteVisual spriteVisual = compositor.CreateSpriteVisual();
            spriteVisual.Shadow = dropShadow;

            ElementCompositionPreview.SetElementChildVisual(BorderExternal, spriteVisual);

            Visual buttonVisual = ElementCompositionPreview.GetElementVisual(Button);
            Visual borderVisual = ElementCompositionPreview.GetElementVisual(Border);

            Button.SizeChanged += (sender1, args1) =>
            {
                spriteVisual.Size = new Vector2((float) Button.ActualWidth, (float) Button.ActualHeight);
            };
        }
    }
}
