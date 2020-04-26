using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace TestAppUWP.AppShell.Core
{
    public class BatchCompletitionAwaiter
    {
        private readonly CompositionScopedBatch _compositionScopedBatch;
        private readonly SemaphoreSlim _semaphoreSlim;

        public BatchCompletitionAwaiter(CompositionScopedBatch compositionScopedBatch)
        {
            _compositionScopedBatch = compositionScopedBatch;
            _semaphoreSlim = new SemaphoreSlim(0);
            _compositionScopedBatch.Completed += BatchCompleted;
        }

        private void BatchCompleted(object source, CompositionBatchCompletedEventArgs args)
        {
            _compositionScopedBatch.Completed -= BatchCompleted;
            _semaphoreSlim.Release();
            _semaphoreSlim.Dispose();
        }

        public Task Completed()
        {
            return _semaphoreSlim.WaitAsync();
        }
    }
}
