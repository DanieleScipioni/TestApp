using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.Samples.Controls
{
    public sealed partial class GroupedListWithDeD
    {
        private readonly List<List<string>> _list;
        private int _dragOverCounter;
        private ItemsStackPanel _listViewItemsPanel;

        public GroupedListWithDeD()
        {
            _list = new List<List<string>>
            {
                new List<string>
                {
                    "aa", "ab", "ac"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                },
                new List<string>
                {
                    "ba", "bb", "bc"
                }
            };

            InitializeComponent();

            ListView.Loaded += (sender, args) =>
            {
                _listViewItemsPanel = (ItemsStackPanel) ListView.ItemsPanelRoot;
            };
            
            ListView.DragItemsStarting += ListViewBase_OnDragItemsStarting;
            ListView.DragItemsCompleted += ListViewBase_OnDragItemsCompleted;
            ListView.DragOver += UIElement_OnDragOver;
            ListView.DragLeave += UIElement_OnDragLeave;
        }

        private void ListViewBase_OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _dragOverCounter = 0;
            _listViewItemsPanel.AreStickyGroupHeadersEnabled = false;
            //ListView.DragItemsCompleted += ListViewBase_OnDragItemsCompleted;
            //ListView.DragOver += UIElement_OnDragOver;
            //ListView.DragLeave += UIElement_OnDragLeave;
        }

        private void ListViewBase_OnDragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            _dragOverCounter = 0;
            _listViewItemsPanel.AreStickyGroupHeadersEnabled = true;
            //ListView.DragItemsCompleted -= ListViewBase_OnDragItemsCompleted;
            //ListView.DragOver -= UIElement_OnDragOver;
            //ListView.DragLeave -= UIElement_OnDragLeave;
        }

        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {
            _dragOverCounter++;
        }

        private void UIElement_OnDragLeave(object sender, DragEventArgs e)
        {
            
        }

        private void ListViewBase_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            
        }
    }
}
