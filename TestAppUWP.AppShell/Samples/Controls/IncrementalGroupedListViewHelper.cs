using System;
using System.Threading.Tasks;
using TestAppUWP.View;
using TestAppUWP.View.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell.Samples.Controls
{
    public class IncrementalGroupedListViewHelper : Behavior
    {
        private ListView _listView;
        private ScrollViewer _scrollViewer;
        private ItemsStackPanel _itemsStackPanel;

        public IncrementalGroupedListViewHelper(ListView listView, ISupportIncrementalLoading supportIncrementalLoading)
        {
            _listView = listView;
            SupportIncrementalLoading = supportIncrementalLoading;
            _listView.Loaded += ListViewOnLoaded;
        }

        #region Behavior

        public IncrementalGroupedListViewHelper() {}

        public override void Attach(DependencyObject dependencyObject)
        {
            if (!(dependencyObject is ListView listView)) return;
            _listView = listView;
            _listView.Loaded += ListViewOnLoaded;
            _listView.Unloaded += ListViewOnUnloaded;
        }

        public override void Detach()
        {
            if (_listView != null)
            {
                _listView.Loaded -= ListViewOnLoaded;
                _listView.Unloaded -= ListViewOnUnloaded;
            }
            _listView = null;
        }

        public ISupportIncrementalLoading SupportIncrementalLoading { get; set; }

        #endregion

        private async void ListViewOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(_listView.ItemsPanelRoot is ItemsStackPanel itemsStackPanel)) return;

            _itemsStackPanel = itemsStackPanel;

            _scrollViewer = VisualTreeHelperUtils.Child<ScrollViewer>(_listView);

            // This event handler loads more items when scrolling.
            _scrollViewer.ViewChanged += ScrollViewerOnViewChanged;

            // This event handler loads more items when size is changed and there is more
            // room in ListView.
            _itemsStackPanel.SizeChanged += ItemsStackPanelOnSizeChanged;

            await LoadMoreItemsAsync(itemsStackPanel);
        }

        private void ListViewOnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer != null) _scrollViewer.ViewChanged -= ScrollViewerOnViewChanged;
            _scrollViewer = null;
            if (_itemsStackPanel != null) _itemsStackPanel.SizeChanged -= ItemsStackPanelOnSizeChanged;
            _itemsStackPanel = null;
        }

        private async void ScrollViewerOnViewChanged(object o, ScrollViewerViewChangedEventArgs eventArgs)
        {
            if (eventArgs.IsIntermediate) return;
            double distanceFromBottom = _itemsStackPanel.ActualHeight - _scrollViewer.VerticalOffset - _scrollViewer.ActualHeight;
            if (distanceFromBottom < 10) // 10 is an arbitrary number
            {
                await LoadMoreItemsAsync(_itemsStackPanel);
            }
        }

        private async void ItemsStackPanelOnSizeChanged(object o, SizeChangedEventArgs eventArgs)
        {
            if (_itemsStackPanel.ActualHeight <= _scrollViewer.ActualHeight)
            {
                await LoadMoreItemsAsync(_itemsStackPanel);
            }
        }


        private async Task LoadMoreItemsAsync(ItemsStackPanel itemsStackPanel)
        {
            if (!SupportIncrementalLoading.HasMoreItems) return;
            // This is to handle the case when the InternalLoadMoreItemsAsync
            // does not fill the entire space of the ListView.
            // This event is needed untill the desired size of itemsStackPanel 
            // is less then the available space.
            itemsStackPanel.LayoutUpdated += OnLayoutUpdated;
            await InternalLoadMoreItemsAsync();
        }

        private async void OnLayoutUpdated(object sender, object e)
        {
            if (_itemsStackPanel.DesiredSize.Height <= _scrollViewer.ActualHeight)
            {
                await InternalLoadMoreItemsAsync();
            }
            else
            {
                _itemsStackPanel.LayoutUpdated -= OnLayoutUpdated;
            }
        }

        private async Task InternalLoadMoreItemsAsync()
        {
            await SupportIncrementalLoading.LoadMoreItemsAsync(5); // 5 is an arbitrary number
        }
    }
}
