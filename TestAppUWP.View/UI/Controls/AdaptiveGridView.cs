using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.View.UI.Controls
{
    public partial class AdaptiveGridView : GridView
    {
        public AdaptiveGridView()
        {
            IsTabStop = false;
            SizeChanged += OnSizeChanged;

            // Prevent issues with higher DPIs and underlying panel.
            UseLayoutRounding = false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject obj, object item)
        {
            base.PrepareContainerForItemOverride(obj, item);
            if (obj is FrameworkElement element)
            {
                RecalculateLayout(element);
            }

            if (obj is ContentControl contentControl)
            {
                contentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                contentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize != e.NewSize) RecalculateLayout(e.NewSize);
        }

        private void RecalculateLayout(FrameworkElement itemContainer)
        {
            double availableSpace = AvailableSpace(new Size(ActualWidth, ActualHeight));
            if (availableSpace <= 0) return;
            int rowsOrColumns = CalculateRowsOrColumns(availableSpace, DesiredMeasure);
            double newMeasure = CalculateItemMeasure(availableSpace, rowsOrColumns);
            Thickness itemContainerMargin = itemContainer.Margin;

            var itemsPanel = (ItemsWrapGrid)ItemsPanelRoot;
            if (itemsPanel == null) return;
            if (itemsPanel.Orientation == Orientation.Horizontal)
            {
                itemContainer.Width = newMeasure - itemContainerMargin.Left - itemContainerMargin.Right;
            }
            else
            {
                itemContainer.Height = newMeasure - itemContainerMargin.Top - itemContainerMargin.Bottom;
            }
        }

        private void RecalculateLayout(Size newSize)
        {
            double availableSpace = AvailableSpace(newSize);
            if (availableSpace <= 0) return;
            int rowsOrColumns = CalculateRowsOrColumns(availableSpace, DesiredMeasure);
            double newMeasure = CalculateItemMeasure(availableSpace, rowsOrColumns);
            
            var itemsPanel = (ItemsWrapGrid)ItemsPanelRoot;
            if (itemsPanel == null) return;

            if (itemsPanel.Orientation == Orientation.Horizontal)
            {
                foreach (UIElement uiElement in itemsPanel.Children)
                {
                    var itemContainer = (FrameworkElement)uiElement;
                    Thickness itemContainerMargin = itemContainer.Margin;
                    itemContainer.Width = newMeasure - itemContainerMargin.Left - itemContainerMargin.Right;
                }
            }
            else
            {
                foreach (UIElement uiElement in itemsPanel.Children)
                {
                    var itemContainer = (FrameworkElement)uiElement;
                    Thickness itemContainerMargin = itemContainer.Margin;
                    itemContainer.Height = newMeasure - itemContainerMargin.Top - itemContainerMargin.Bottom;
                }
            }
        }

        private double AvailableSpace(Size newSize)
        {
            var itemsPanel = (ItemsWrapGrid)ItemsPanelRoot;
            if (itemsPanel == null) return 0;

            return itemsPanel.Orientation == Orientation.Horizontal ? newSize.Width : newSize.Height;
        }

        protected virtual double CalculateItemMeasure(double availableSpace, int rowsOrColumns)
        {
            var itemsPanel = (ItemsWrapGrid)ItemsPanelRoot;
            if (itemsPanel == null) return 0;

            Orientation orientation = itemsPanel.Orientation;
            Thickness itemsPanelMargin = itemsPanel.Margin;
            double panelMargin;
            double padding;
            double border;
            if (orientation == Orientation.Horizontal)
            {
                panelMargin = itemsPanelMargin.Left + itemsPanelMargin.Right;
                padding = Padding.Left + Padding.Right;
                border = BorderThickness.Left + BorderThickness.Right;
            }
            else
            {
                panelMargin = itemsPanelMargin.Top + itemsPanelMargin.Bottom;
                padding = Padding.Top + Padding.Bottom;
                border = BorderThickness.Top + BorderThickness.Bottom;
            }
            return (availableSpace - padding - panelMargin - border) / rowsOrColumns;
        }

        private static int CalculateRowsOrColumns(double containerSpace, double itemMeasure)
        {
            if (double.IsNaN(itemMeasure)) itemMeasure = containerSpace;
            var columns = (int)Math.Floor(containerSpace / itemMeasure);
            return columns == 0 ? 1 : columns;
        }
    }
}
