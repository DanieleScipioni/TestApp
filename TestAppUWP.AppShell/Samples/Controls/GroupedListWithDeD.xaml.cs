using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestAppUWP.AppShell.Samples.Controls
{
    public sealed partial class GroupedListWithDeD
    {
        private readonly IncremetalStringGroups _availableGroups;
        private readonly ObservableCollection<StringGroup> _choosenGroups;

        public GroupedListWithDeD()
        {
            _availableGroups = new IncremetalStringGroups();
            _choosenGroups = new ObservableCollection<StringGroup>();
            InitializeComponent();

            AvailableDropAction = AvailableDropDelegate;
            ChoosenDropAction = ChoosenDropDelegate;

             //_ = new IncrementalGroupedListViewHelper(IncrementalGroupedListView, _availableGroups);
        }

        public Action<List<string>> AvailableDropAction { get; set; }
        private void AvailableDropDelegate(List<string> items)
        {
            foreach (string item in items)
            {
                RemoveFromGroup(item, _choosenGroups);
            }
        }

        public Action<List<string>> ChoosenDropAction { get; set; }
        public void ChoosenDropDelegate(List<string> items)
        {
            foreach (string item in items)
            {
                AddToGroup(item, _choosenGroups);
            }
        }

        private void AddToGroup(string item, ObservableCollection<StringGroup> groups)
        {
            char groupKey = item[0];
            StringGroup stringGroup = groups.FirstOrDefault(l => l.Count > 0 && l[0][0] == groupKey);
            if (stringGroup != null)
            {
                stringGroup.Add(item);
            }
            else
            {
                _choosenGroups.Add(new StringGroup(groupKey.ToString()) { item });
            }
        }

        private void RemoveFromGroup(string item, ObservableCollection<StringGroup> groups)
        {
            char groupKey = item[0];
            StringGroup stringGroup = groups.FirstOrDefault(l => l.Count > 0 && l[0][0] == groupKey);
            if (stringGroup == null) return;
            stringGroup.Remove(item);
            if (stringGroup.Count == 0) groups.Remove(stringGroup);
        }
    }

    public class StringGroup : ObservableCollection<string>
    {
        public readonly string Key;

        public StringGroup(string key)
        {
            Key = key;
        }
    }
}
