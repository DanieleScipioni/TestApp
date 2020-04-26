using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestAppUWP.AppShell.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.UserControls
{
    public class StringWrapper : GroupedItem
    {
        public string String { get; set; }
    }

    public sealed partial class ListUserControl
    {
        private readonly CollectionViewSource _collectionViewSource;

        public ListUserControl()
        {
            InitializeComponent();

            _collectionViewSource = new CollectionViewSource
            {
                IsSourceGrouped = true,
            };

            var listViewGroupedDragDrop = new ListViewGroupedDragDrop(ListView);
        }


        private void ButtonBase_OnClickk(object sender, RoutedEventArgs e)
        {
            var collectionView = ListView.ItemsSource as ICollectionView;
            ItemCollection listViewItems = ListView.Items;
            testResult.Text = listViewItems.Count.ToString();
        }

        private void Restart_OnClick(object sender, RoutedEventArgs e)
        {
            var collection = new ObservableCollection<StringWrapper>
            {
                new StringWrapper {String = "1_A"},
                new StringWrapper {String = "1_B"},
                new StringWrapper {String = "1_C"},
                new StringWrapper {String = "2_D"},
                new StringWrapper {String = "2_E"},
                new StringWrapper {String = "2_F"},
                new StringWrapper {String = "3_G"},
                new StringWrapper {String = "3_H"},
                new StringWrapper {String = "3_I"},
                new StringWrapper {String = "4_J"},
                new StringWrapper {String = "4_K"},
                new StringWrapper {String = "4_L"},
                new StringWrapper {String = "5_M"},
                new StringWrapper {String = "5_N"},
                new StringWrapper {String = "5_O"}
            };

            List<Group> list = (from item in collection
                group item by item.String.Substring(0, 1)
                into grp
                select new Group(grp.Key, grp)).ToList();

            //List<Group> list = (from item in collection group item by item.String.Substring(0, 1) into grp select grp)
            //    .Select(stringWrappers => new Group(stringWrappers.Key,
            //        stringWrappers)).ToList();
            _collectionViewSource.Source = list;
        }
    }

}
