using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestAppUWP.AppShell.Samples.Controls
{
    public sealed partial class GroupedListWithDeD
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly ObservableCollection<Group> _availableGroups;
        private readonly ObservableCollection<Group> _choosenGroups;

        public GroupedListWithDeD()
        {
            _availableGroups = new ObservableCollection<Group>
            {
                new Group("a")
                {
                    "aa", "ab", "ac"
                },
                new Group("b")
                {
                    "ba", "bb", "bc"
                },
                new Group("c")
                {
                    "ca", "cb", "Cc"
                },
                new Group("d")
                {
                    "da", "db", "dc"
                },
                new Group("e")
                {
                    "ea", "eb", "ec"
                }
            };
            _choosenGroups = new ObservableCollection<Group>();
            InitializeComponent();

            AvailableDropAction = AvailableDropDelegate;
            ChoosenDropAction = ChoosenDropDelegate;
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

        private void AddToGroup(string item, ObservableCollection<Group> groups)
        {
            char groupKey = item[0];
            Group group = groups.FirstOrDefault(l => l.Count > 0 && l[0][0] == groupKey);
            if (group != null)
            {
                group.Add(item);
            }
            else
            {
                _choosenGroups.Add(new Group(groupKey.ToString()) {item});
            }
        }

        private void RemoveFromGroup(string item, ObservableCollection<Group> groups)
        {
            char groupKey = item[0];
            Group group = groups.FirstOrDefault(l => l.Count > 0 && l[0][0] == groupKey);
            if (group == null) return;
            group.Remove(item);
            if (group.Count == 0) groups.Remove(group);
        }
    }

    public class Group : ObservableCollection<string>
    {
        public readonly string Key;

        public Group(string key)
        {
            Key = key;
        }
    }
}
