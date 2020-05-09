using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using TestAppUWP.View.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

namespace TestAppUWP.AppShell.Samples.Controls
{
    internal class ListViewDdBehavior : Behavior
    {
        private ListView _listView;
        private ItemsStackPanel _listViewItemsPanelRoot;
        private bool _originalAreStickyGroupHeadersEnabled;

        public override void Attach(DependencyObject dependencyObject)
        {
            _listView = (ListView) dependencyObject;
            _listView.Loaded += ListViewOnLoaded;
            _listView.Unloaded += ListViewOnUnloaded;
        }

        public override void Detach()
        {
        }

        private void ListViewOnLoaded(object sender, RoutedEventArgs e)
        {
            _listView.DragItemsStarting += ListViewBase_OnDragItemsStarting;
            if (_listView.ItemsPanelRoot is ItemsStackPanel itemsStackPanel)
            {
                _listViewItemsPanelRoot = itemsStackPanel;
            }
            _listView.DragEnter += ListViewOnDragEnter;
            _listView.DragLeave += ListViewOnDragLeave;
        }

        private void ListViewOnUnloaded(object sender, RoutedEventArgs e)
        {
            _listView.DragItemsStarting -= ListViewBase_OnDragItemsStarting;
            _listView.DragEnter -= ListViewOnDragEnter;
            _listView.DragLeave -= ListViewOnDragLeave;
        }

        private void ListViewBase_OnDragItemsStarting(object sender, DragItemsStartingEventArgs args)
        {
            _originalAreStickyGroupHeadersEnabled = _listViewItemsPanelRoot.AreStickyGroupHeadersEnabled;
            _listViewItemsPanelRoot.AreStickyGroupHeadersEnabled = false;
            _listView.DragItemsCompleted += ListViewOnDragItemsCompleted;

            var items = args.Items.Cast<string>().ToList();
            args.Data.Properties.Add("items", items);
        }

        private void ListViewOnDragOver(object sender, DragEventArgs e)
        {
            var items = (List<string>) e.Data.Properties["items"];

            if (items.Count > 0)
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        private void ListViewOnDrop(object sender, DragEventArgs e)
        {
            _listView.Drop -= ListViewOnDragOver;
            _listView.Drop -= ListViewOnDrop;

            var items = (List<string>) e.Data.Properties["items"];
            Drop?.Invoke(items);
        }

        private void ListViewOnDragEnter(object sender, DragEventArgs e)
        {
            _listView.DragOver += ListViewOnDragOver;
            _listView.Drop += ListViewOnDrop;
        }

        private void ListViewOnDragLeave(object sender, DragEventArgs e)
        {
            _listView.DragOver -= ListViewOnDragOver;
            _listView.Drop -= ListViewOnDrop;
        }

        private void ListViewOnDragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            _listViewItemsPanelRoot.AreStickyGroupHeadersEnabled = _originalAreStickyGroupHeadersEnabled;
            _listView.DragOver -= ListViewOnDragOver;
            _listView.Drop -= ListViewOnDrop;
            _listView.DragItemsCompleted -= ListViewOnDragItemsCompleted;
        }

        public Action<List<string>> Drop { get; set; }
    }
}