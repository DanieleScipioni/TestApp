using System.Threading.Tasks;

namespace TestAppUWP.Samples.Animations.DropShadowStuff
{
    public sealed partial class DropShadowPage
    {
        public DropShadowPage()
        {
            InitializeComponent();
            Loaded += async (sender, args) =>
            {
                await DropShadowMethod1();
            };
        }

        private async Task DropShadowMethod1()
        {
            var sun = new Sun();
            await sun.DrawShadow(CentralGrid, CentralGridShadowHost);
            await sun.DrawShadow(Button1, ButtonsShadowHost);
            await sun.DrawShadow(Button2, ButtonsShadowHost);
        }
    }
}
