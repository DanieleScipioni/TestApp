using System.Threading.Tasks;

namespace TestAppUWP.Samples.Map
{
    internal class AsyncManualResetEvent
    {
        private readonly object _lock = new object();

        private TaskCompletionSource<object> _taskCompletionSource;

        public AsyncManualResetEvent(bool initialState)
        {
            _taskCompletionSource = new TaskCompletionSource<object>();
            if (initialState) _taskCompletionSource.SetResult(null);
        }

        public void Set()
        {
            if (_taskCompletionSource.Task.IsCompleted) return;
            lock (_lock)
            {
                if (_taskCompletionSource.Task.IsCompleted) return;
                _taskCompletionSource.SetResult(null);
            }
        }

        public void Reset()
        {
            if (!_taskCompletionSource.Task.IsCompleted) return;
            lock (_lock)
            {
                if (!_taskCompletionSource.Task.IsCompleted) return;
                _taskCompletionSource = new TaskCompletionSource<object>();
            }
        }

        public Task Wait()
        {
            return _taskCompletionSource.Task;
        }
    }
}