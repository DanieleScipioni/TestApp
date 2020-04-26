﻿using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TestAppUWP.AppShell.Samples.Animations.DropShadowStuff
{
    public class Sun
    {
        private readonly Dictionary<FrameworkElement, SpriteVisual> _visualByUiElement;
        private readonly Dictionary<FrameworkElement, List<FrameworkElement>> _shadowSourceByShadowHost;

        public Sun()
        {
            _visualByUiElement = new Dictionary<FrameworkElement, SpriteVisual>();
            _shadowSourceByShadowHost = new Dictionary<FrameworkElement, List<FrameworkElement>>();
        }

        private void ShadowHostOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var shadowHost = (FrameworkElement) sender;
            List<FrameworkElement> shadowSources = _shadowSourceByShadowHost[shadowHost];
            foreach (FrameworkElement shadowSource in shadowSources)
            {
                SpriteVisual spriteVisual = _visualByUiElement[shadowSource];
                spriteVisual.Offset = VisualOffset(shadowSource, shadowHost);
            }
        }

        public async Task DrawShadow(FrameworkElement shadowSource, FrameworkElement shadowHost)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(shadowHost).Compositor;

            List<FrameworkElement> frameworkElements;
            ContainerVisual shadowHostContainerVisual;
            if (_shadowSourceByShadowHost.ContainsKey(shadowHost))
            {
                frameworkElements = _shadowSourceByShadowHost[shadowHost];
                shadowHostContainerVisual = (ContainerVisual) ElementCompositionPreview.GetElementChildVisual(shadowHost);
            }
            else
            {
                frameworkElements = new List<FrameworkElement>();
                _shadowSourceByShadowHost.Add(shadowHost, frameworkElements);
                shadowHostContainerVisual = compositor.CreateContainerVisual();
                ElementCompositionPreview.SetElementChildVisual(shadowHost, shadowHostContainerVisual);
                shadowHost.SizeChanged += ShadowHostOnSizeChanged;
            }

            frameworkElements.Add(shadowSource);

            shadowSource.SizeChanged += ShadowSourceOnSizeChanged;

            DropShadow dropShadow = compositor.CreateDropShadow();
            dropShadow.BlurRadius = 5;
            dropShadow.Offset = new Vector3(10, 10, 0);
            dropShadow.Color = Colors.DarkSlateGray;
            dropShadow.Mask = await ShadowMask(shadowSource, compositor);
            dropShadow.SourcePolicy = CompositionDropShadowSourcePolicy.Default;

            SpriteVisual visual = compositor.CreateSpriteVisual();
            visual.Size = new Vector2((float)shadowSource.ActualWidth, (float)shadowSource.ActualHeight);
            visual.Shadow = dropShadow;
            visual.Offset = VisualOffset(shadowSource, shadowHost);

            shadowHostContainerVisual.Children.InsertAtTop(visual);

            _visualByUiElement.Add(shadowSource, visual);
        }

        private async void ShadowSourceOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var shadowSource = (FrameworkElement) sender;
            Compositor compositor = ElementCompositionPreview.GetElementVisual(shadowSource).Compositor;
            SpriteVisual visual = _visualByUiElement[shadowSource];
            visual.Size = new Vector2((float)shadowSource.ActualWidth, (float)shadowSource.ActualHeight);
            var dropShadow = (DropShadow) visual.Shadow;
            dropShadow.Mask = await ShadowMask(shadowSource, compositor);
        }

        private async Task<CompositionBrush> ShadowMask(UIElement uiElement, Compositor compositor)
        {
            CompositionBrush compositionBrush;
            switch (uiElement)
            {
                case Shape shape:
                    compositionBrush = shape.GetAlphaMask();
                    break;
                case TextBlock textBlock:
                    compositionBrush = textBlock.GetAlphaMask();
                    break;
                case Image image:
                    compositionBrush = image.GetAlphaMask();
                    break;
                default:
                    CompositionDrawingSurface drawingSurface =
                        await CompositionDrawingSurfaceFacade2.GetCompositionDrawingSurface(uiElement, compositor);
                    compositionBrush = compositor.CreateSurfaceBrush(drawingSurface);
                    break;
            }
            return compositionBrush;
        }

        private static Vector3 VisualOffset(UIElement shadowSource, FrameworkElement shoadowHost)
        {
            GeneralTransform transformToVisual = shadowSource.TransformToVisual(shoadowHost);
            Point transformPoint = transformToVisual.TransformPoint(new Point(0, 0));
            return new  Vector3((float) transformPoint.X, (float) transformPoint.Y, 0);
        }
    }
}