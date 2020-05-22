using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace TestAppUWP.AppShell.Samples.Controls
{
    public class IncremetalStringGroups : ObservableCollection<StringGroup>, ISupportIncrementalLoading
    {
        private readonly Random _random = new Random();

        private char _nextLetter = 'A';

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var stringGroup = new StringGroup(_nextLetter.ToString());
            int countToAdd = _random.Next(5, 10);
            for (var counter = 0; counter < countToAdd; counter++)
            {
                stringGroup.Add($"{_nextLetter} {counter}");
            }
            Add(stringGroup);

            _nextLetter = (char)(_nextLetter + 1);
            return AsyncInfo.Run(ct => Task.FromResult(new LoadMoreItemsResult { Count = (uint)Count }));
        }

        public bool HasMoreItems => _nextLetter <= 'z';
    }
}
