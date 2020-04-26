﻿using System.Threading.Tasks;
using Windows.UI;

namespace TestAppUWP.AppShell.Samples.Animations.DropShadowStuff
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
            var sun = new Sun(Colors.DarkSlateGray, 10, 10);
            await sun.DrawShadow(ButtonsShadowHost, CentralGridShadowHost, Colors.BurlyWood, 20f, 20f);
            await sun.DrawShadow(Button1, ButtonsShadowHost);
            await sun.DrawShadow(Button2, ButtonsShadowHost);
        }
    }
}
