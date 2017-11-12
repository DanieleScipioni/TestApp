using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace AppServiceServerBgTask
{
    public sealed class AppServiceServerBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            if (!(taskInstance.TriggerDetails is AppServiceTriggerDetails appServiceTriggerDetails)) return;

            AppServiceConnection appServiceConnection = appServiceTriggerDetails.AppServiceConnection;
            appServiceConnection.RequestReceived += async (sender, args) =>
            {
                AppServiceDeferral appServiceDeferral = args.GetDeferral();

                AppServiceRequest appServiceRequest = args.Request;
                ValueSet request = appServiceRequest.Message;

                await appServiceRequest.SendResponseAsync(new ValueSet {{"count", request.Count}});

                appServiceDeferral.Complete();
            };

        }
    }
}
