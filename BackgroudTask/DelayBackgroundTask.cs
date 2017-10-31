using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace TestAppBackgroundTask
{
    public sealed class DelayBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundWorkCostValue cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            if (cost == BackgroundWorkCostValue.High) return;

            if (taskInstance.TriggerDetails is ApplicationTriggerDetails applicationTriggerDetails)
            {
                ValueSet valueSet = applicationTriggerDetails.Arguments;
                int valueSetCount = valueSet.Count;
            }

            using (var cts = new CancellationTokenSource())
            {
                taskInstance.Canceled += (sender, reason) =>
                {
                    cts.Cancel();
                };

                BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
                try
                {
                    await InternalRun(taskInstance, cts.Token);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private static async Task InternalRun(IBackgroundTaskInstance taskInstance, CancellationToken ct)
        {
            for (var i = 0; i < 5; i++)
            {
                if (ct.IsCancellationRequested) return;
                await Task.Delay(1000, ct);
                taskInstance.Progress++;
            }
        }
    }
}
