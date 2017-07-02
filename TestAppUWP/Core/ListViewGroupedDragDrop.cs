using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Core
{
    public class GroupedItem
    {
        public Group Group { get; set; }
    }

    public class Group : ObservableCollection<GroupedItem>
    {
        public string GroupName { get; }

        public Group(string groupName, IEnumerable<GroupedItem> collection) : base(collection)
        {
            GroupName = groupName;
            foreach (GroupedItem groupedItem in collection)
            {
                groupedItem.Group = this;
            }
        }
    }

    public class ListViewGroupedDragDrop
    {
        private readonly ListView _listView;
        private readonly Color _colorValue;

        private List<GroupedItem> _dragGroupedItems;

        private Tuple<ListViewItem, int> _lastOverItemAndIndex;
        private Brush _lastBorderBrush;
        private PlacementMode _lastPlacementMode;
        private ScrollMode _lastScrollMode;

        public ListViewGroupedDragDrop(ListView listView)
        {
            var uiSettings = new UISettings();
            _colorValue = uiSettings.GetColorValue(UIColorType.Accent);

            _listView = listView;
            _lastOverItemAndIndex = new Tuple<ListViewItem, int>(null, -1);
            _lastPlacementMode = PlacementMode.Mouse;
            
            listView.DragItemsStarting += ListViewOnDragItemsStarting;
            listView.DragItemsCompleted += ListViewOnDragItemsCompleted;
            listView.DragEnter += ListViewOnDragEnter;
            listView.DragOver += ListViewOnDragOver;
            listView.DragLeave += ListViewOnDragLeave;
            listView.Drop += ListViewOnDrop;
            listView.DropCompleted += ListViewOnDropCompleted;

        }

        private async void ListViewOnDragEnter(object sender, DragEventArgs dragEventArgs)
        {
            var listView = sender as ListView;
            if (listView == null) return;

            _lastScrollMode = ScrollViewer.GetVerticalScrollMode(listView);
            ScrollViewer.SetVerticalScrollMode(listView, ScrollMode.Disabled);

            Tuple<ListViewItem, int> currentOverItemAndIndex = GetCurrentOverItemAndIndex(dragEventArgs, listView);
            ListViewItem currentOverListViewItem = currentOverItemAndIndex.Item1;

            if (currentOverListViewItem == null) return;

            DragOperationDeferral dragOperationDeferral = dragEventArgs.GetDeferral();

            BitmapImage bitmapImage = await GetBitmapImage(currentOverListViewItem);
            dragEventArgs.DragUIOverride?.SetContentFromBitmapImage(bitmapImage);

            foreach (GroupedItem groupedItem in _dragGroupedItems)
            {
                groupedItem.Group?.Remove(groupedItem);
                groupedItem.Group = null;
            }

            dragEventArgs.Handled = true;
            dragOperationDeferral.Complete();
        }

        private static async Task<BitmapImage> GetBitmapImage(UIElement uiElement)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(uiElement);
            var bitmapImage = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(await renderTargetBitmap.GetPixelsAsync());
                stream.Seek(0);
                bitmapImage.SetSource(stream);
            }
            return bitmapImage;
        }

        private void ListViewOnDragOver(object sender, DragEventArgs dragEventArgs)
        {
            dragEventArgs.Handled = true;

            var listView = sender as ListView;
            if (listView == null) return;

            dragEventArgs.AcceptedOperation = DataPackageOperation.Move;

            Tuple<ListViewItem, int> currentOverItemAndIndex = GetCurrentOverItemAndIndex(dragEventArgs, listView);

            if (currentOverItemAndIndex == null) return;

            if (currentOverItemAndIndex.Item2 != _lastOverItemAndIndex.Item2 && currentOverItemAndIndex.Item1 != null)
            {
                if (_lastOverItemAndIndex?.Item1 != null)
                {
                    _lastOverItemAndIndex.Item1.BorderBrush = _lastBorderBrush;
                    _lastOverItemAndIndex.Item1.BorderThickness = new Thickness(0, 0, 0, 0);
                }

                _lastBorderBrush = currentOverItemAndIndex.Item1?.BorderBrush;
                _lastOverItemAndIndex = currentOverItemAndIndex;
            }

            if (currentOverItemAndIndex.Item1 == null) return;

            Point point = dragEventArgs.GetPosition(currentOverItemAndIndex.Item1);
            PlacementMode currentPlacementMode = point.Y < currentOverItemAndIndex.Item1.ActualHeight / 2 ? PlacementMode.Top : PlacementMode.Bottom;
            if (currentPlacementMode == _lastPlacementMode) return;

            _lastPlacementMode = currentPlacementMode;
            currentOverItemAndIndex.Item1.BorderBrush = new SolidColorBrush(_colorValue);

            currentOverItemAndIndex.Item1.BorderThickness = currentPlacementMode == PlacementMode.Top
                ? new Thickness(0, 1, 0, 0)
                : new Thickness(0, 0, 0, 1);
        }

        private static Tuple<ListViewItem, int> GetCurrentOverItemAndIndex(DragStartingEventArgs dragStartingEventArgs, ListView listView)
        {
            Point position = dragStartingEventArgs.GetPosition(Window.Current.Content);
            return CurrentOverItemAndIndex(listView, position);
        }

        private static Tuple<ListViewItem, int> GetCurrentOverItemAndIndex(DragEventArgs dragEventArgs, ListView listView)
        {
            Point position = dragEventArgs.GetPosition(Window.Current.Content);
            return CurrentOverItemAndIndex(listView, position);
        }

        private void ListViewOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            var listView = sender as ListView;
            if (listView == null) return;

            ScrollViewer.SetVerticalScrollMode(listView, _lastScrollMode);

            if (_lastOverItemAndIndex.Item1 == null) return;

            var groupedItem = (GroupedItem) _listView.ItemFromContainer(_lastOverItemAndIndex.Item1);
            int indexOf = groupedItem.Group.IndexOf(groupedItem);
            if (_lastPlacementMode == PlacementMode.Bottom) indexOf++;

            for (int i = _dragGroupedItems.Count - 1; i >= 0; i--)
            {
                GroupedItem dragGroupedItem = _dragGroupedItems[i];
                dragGroupedItem.Group = groupedItem.Group;
                groupedItem.Group.Insert(indexOf, dragGroupedItem);
            }

            _lastOverItemAndIndex.Item1.BorderBrush = _lastBorderBrush;
            _lastOverItemAndIndex.Item1.BorderThickness = new Thickness(0);

            _lastOverItemAndIndex = new Tuple<ListViewItem, int>(null, -1);
            _lastBorderBrush = null;
            _lastPlacementMode = PlacementMode.Mouse;

            _dragGroupedItems = null;
            dragEventArgs.Handled = true;
        }

        private void ListViewOnDragLeave(object sender, DragEventArgs dragEventArgs)
        {
            var listView = sender as ListView;
            if (listView == null) return;

            if (_lastOverItemAndIndex.Item1 == null) return;

            ScrollViewer.SetVerticalScrollMode(listView, _lastScrollMode);

            _lastOverItemAndIndex.Item1.BorderBrush = _lastBorderBrush;
            _lastOverItemAndIndex.Item1.BorderThickness = new Thickness(0);

            _lastOverItemAndIndex = new Tuple<ListViewItem, int>(null, -1);
            _lastBorderBrush = null;
            _lastPlacementMode = PlacementMode.Mouse;

            dragEventArgs.Handled = true;
        }

        private void ListViewOnDropCompleted(UIElement sender, DropCompletedEventArgs dropCompletedEventArgs)
        {
            _dragGroupedItems = null;
        }

        private void ListViewOnDragItemsStarting(object sender, DragItemsStartingEventArgs dragItemsStartingEventArgs)
        {
            var listView = sender as ListView;
            if (listView == null) return;
            _dragGroupedItems = dragItemsStartingEventArgs.Items.Cast<GroupedItem>().ToList();
        }

        private static void ListViewOnDragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs dragItemsCompletedEventArgs)
        {
        }

        private static Tuple<ListViewItem, int> CurrentOverItemAndIndex(ListView listView, Point position)
        {
            IEnumerable<UIElement> elements =
                VisualTreeHelper.FindElementsInHostCoordinates(position, listView);

            foreach (UIElement element in elements)
            {
                var listViewItem = element as ListViewItem;
                if (listViewItem == null) continue;

                int itemIndex = listView.IndexFromContainer(listViewItem);
                if (itemIndex == -1) continue;

                return new Tuple<ListViewItem, int>((ListViewItem)element, itemIndex);
            }
            return new Tuple<ListViewItem, int>(null, -1);
        }

    }
}
