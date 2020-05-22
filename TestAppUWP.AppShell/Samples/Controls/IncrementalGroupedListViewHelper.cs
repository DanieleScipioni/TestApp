using System;
using System.Threading.Tasks;
using TestAppUWP.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell.Samples.Controls
{
    public class IncrementalGroupedListViewHelper
    {
        private readonly ListView _listView;
        private readonly ISupportIncrementalLoading _supportIncrementalLoading;
        
        private ScrollViewer _scrollViewer;
        private ItemsStackPanel _itemsStackPanel;


        public IncrementalGroupedListViewHelper(ListView listView, ISupportIncrementalLoading supportIncrementalLoading)
        {
            _listView = listView;
            _supportIncrementalLoading = supportIncrementalLoading;
            _listView.Loaded += ListViewOnLoaded;
        }

        private async void ListViewOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(_listView.ItemsPanelRoot is ItemsStackPanel itemsStackPanel)) return;

            _itemsStackPanel = itemsStackPanel;

            _scrollViewer = VisualTreeHelperUtils.Child<ScrollViewer>(_listView);

            _scrollViewer.ViewChanged += async (o, eventArgs) =>
            {
                if (eventArgs.IsIntermediate) return;
                await LoadMoreItemsAsync(itemsStackPanel, _scrollViewer);
            };

            itemsStackPanel.SizeChanged += async (o, eventArgs) =>
            {
                if (itemsStackPanel.ActualHeight <= _scrollViewer.ActualHeight)
                {
                    await LoadMoreItemsAsync(itemsStackPanel, _scrollViewer);
                }
            };

            itemsStackPanel.LayoutUpdated += OnLayoutUpdated;

            await LoadMoreItemsAsync(itemsStackPanel, _scrollViewer);
        }

        private async void OnLayoutUpdated(object sender, object e)
        {
            if (_itemsStackPanel.DesiredSize.Height <= _scrollViewer.ActualHeight)
            {
                await LoadMoreItemsAsync(_itemsStackPanel, _scrollViewer);
            }
            else
            {
                _itemsStackPanel.LayoutUpdated -= OnLayoutUpdated;
            }
        }

        private async Task LoadMoreItemsAsync(ItemsStackPanel itemsStackPanel, ScrollViewer scrollViewer)
        {
            if (!_supportIncrementalLoading.HasMoreItems) return;
            double distanceFromBottom = itemsStackPanel.ActualHeight - scrollViewer.VerticalOffset - scrollViewer.ActualHeight;
            if (distanceFromBottom < 10)
            {
                await _supportIncrementalLoading.LoadMoreItemsAsync(5);
            }
        }
    }
}
