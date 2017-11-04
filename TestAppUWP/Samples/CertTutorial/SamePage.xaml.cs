using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TestAppBackgroundTask;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Store;
using Windows.Storage.AccessCache;

namespace TestAppUWP.Samples.CertTutorial
{
    public sealed partial class SamePage
    {
        private readonly ApplicationTrigger _applicationTrigger;

        public SamePage()
        {
            InitializeComponent();

            _applicationTrigger = new ApplicationTrigger();

            if (IsTypePresent != null)
            {
                IsTypePresent.Text =
                    $"ApiInformation.IsTypePresent Windows.Phone.UI.Input.HardwareButtons = {ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")}";
            }

            ResourceContext resourceContext = ResourceContext.GetForCurrentView();
            List<string> list = resourceContext.QualifierValues.Keys.ToList();

            Loaded += async (sender, args) =>
            {
                BackgroundTaskRegistration backgroundTaskRegistration = await RegisterBackgroudTask(_applicationTrigger);
                if (backgroundTaskRegistration != null)
                {
                    backgroundTaskRegistration.Progress += delegate (BackgroundTaskRegistration registration, BackgroundTaskProgressEventArgs eventArgs)
                    {
                        // ReSharper disable once UnusedVariable
                        IAsyncAction ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            BackgroundTextBlock.Text = eventArgs.Progress.ToString();
                        });
                    };
                    backgroundTaskRegistration.Completed += delegate (BackgroundTaskRegistration registration, BackgroundTaskCompletedEventArgs eventArgs)
                    {
                        // ReSharper disable once UnusedVariable
                        IAsyncAction ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            BackgroundTextBlock.Text = "Completed";
                        });
                    };
                }

                if (MinSizeTExtBlock != null)
                {
                    Window currentWindow = Window.Current;
                    MinSizeTExtBlock.Text = $"{currentWindow.Bounds.Width} x {currentWindow.Bounds.Height}";
                    currentWindow.SizeChanged += (o, eventArgs) =>
                    {
                        Size eventArgsSize = eventArgs.Size;
                        MinSizeTExtBlock.Text = $"{eventArgsSize.Width} x {eventArgsSize.Height}";
                    };
                }

                DataTransferManager t = DataTransferManager.GetForCurrentView();
                t.DataRequested += (dataSender, dataRequestedEventArgs) =>
                {
                    DataPackage r = dataRequestedEventArgs.Request.Data;
                    r.Properties.ApplicationName = "Stoca";
                    r.Properties.Description = "stoca description";
                    r.Properties.Title = "Stoca title";
                    r.SetText("stoca shared");
                };
                t.ShareProvidersRequested += (dataSender, shareProvidersRequestedEventArgs) =>
                {
                };
                t.TargetApplicationChosen += (dataSender, targetApplicationChosenEventArgs) =>
                {
                };
            };

            Unloaded += (sender, args) =>
            {
                IList<VisualStateGroup> visualStateGroups = VisualStateManager.GetVisualStateGroups((FrameworkElement)Content);

                IEnumerable<OrientationStateTrigger> orientationStateTriggers = visualStateGroups.SelectMany(visualStateGroup => visualStateGroup.States)
                    .SelectMany(visualState => visualState.StateTriggers)
                    .Where(stateTriggerBase => stateTriggerBase is OrientationStateTrigger)
                    .Cast<OrientationStateTrigger>();
                foreach (OrientationStateTrigger orientationStateTrigger in orientationStateTriggers)
                {
                    orientationStateTrigger.Dispose();
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string textBoxText = (string)e.Parameter ?? string.Empty;
            if (TextBlock != null) TextBlock.Text = textBoxText;
            if (TextBox != null) TextBox.Text = textBoxText;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CertTutorial.Instance.SetNavigationParam(TextBox.Text);
        }

        private void MinSize_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(200, 200));
        }

        private void SetSize_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TryResizeView(new Size(555, 666));
        }

        private async void RunBackgroudTask(object sender, RoutedEventArgs e)
        {
            var valueSet = new ValueSet();
            var cts = new CancellationTokenSource();
            valueSet.Add("CT", 1);
            ApplicationTriggerResult applicationTriggerResult = await _applicationTrigger.RequestAsync(valueSet);
            switch (applicationTriggerResult)
            {
                case ApplicationTriggerResult.Allowed:
                    break;
                case ApplicationTriggerResult.CurrentlyRunning:
                    break;
                case ApplicationTriggerResult.DisabledByPolicy:
                    break;
                case ApplicationTriggerResult.UnknownError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async Task<BackgroundTaskRegistration> RegisterBackgroudTask(IBackgroundTrigger backgroundTrigger)
        {
            BackgroundAccessStatus backgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus();
            if (backgroundAccessStatus == BackgroundAccessStatus.Unspecified)
                backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            switch (backgroundAccessStatus)
            {
                // cancelled
                case BackgroundAccessStatus.Unspecified:
                    return null;

                // allowed
                case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity:
                case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity:
                case BackgroundAccessStatus.AlwaysAllowed:
                case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                    break;

                // denied
                case BackgroundAccessStatus.Denied:
                case BackgroundAccessStatus.DeniedBySystemPolicy:
                case BackgroundAccessStatus.DeniedByUser:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IReadOnlyDictionary<Guid, IBackgroundTaskRegistration> allTasks = BackgroundTaskRegistration.AllTasks;

            allTasks.Where(bt => bt.Value.Name == nameof(DelayBackgroundTask))
                .Select(x => x.Value).FirstOrDefault()?.Unregister(true);

            var backgroundTaskBuilder = new BackgroundTaskBuilder
            {
                Name = nameof(DelayBackgroundTask),
                CancelOnConditionLoss = false,
                TaskEntryPoint = typeof(DelayBackgroundTask).ToString()
            };

            backgroundTaskBuilder.SetTrigger(backgroundTrigger);
            return backgroundTaskBuilder.Register();
        }

        private async void CreateSecondaryTile(object sender, RoutedEventArgs e)
        {
            bool exists = SecondaryTile.Exists("1234");
            if (!exists)
            {
                var secondaryTile = new SecondaryTile("1234")
                {
                    DisplayName = "Display Name",
                    Arguments = "Arguments",
                    //tileData.VisualElements.ShowNameOnSquare150x150Logo = true;
                };
                secondaryTile.VisualElements.Square150x150Logo =
                    new Uri("ms-appx:///assets/Square150x150Logo.scale-200.png");
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                GeneralTransform buttonTransform = ((FrameworkElement)sender).TransformToVisual(null);
                Point point = buttonTransform.TransformPoint(new Point());

                await secondaryTile.RequestCreateAsync(point);
            }
        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        private async void CreateToast(object sender, RoutedEventArgs e)
        {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

            xml.DocumentElement.SetAttribute("launch", "Args");
            xml.GetElementsByTagName("text")[0].AppendChild(xml.CreateTextNode("ciao ciao"));

            await xml.SaveToFileAsync(await ApplicationData.Current.LocalFolder.CreateFileAsync("xml.xml",
                    CreationCollisionOption.OpenIfExists));

            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xml));
        }

        private void CreateBadge(object sender, RoutedEventArgs e)
        {
            const BadgeTemplateType badgeTemplateType = BadgeTemplateType.BadgeGlyph;
            XmlDocument templateContent = BadgeUpdateManager.GetTemplateContent(badgeTemplateType);
            (templateContent.GetElementsByTagName("badge").FirstOrDefault() as XmlElement)?.SetAttribute("value", "alarm");

            BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            var badgeNotification = new BadgeNotification(templateContent)
            {
                ExpirationTime = new DateTimeOffset(DateTime.UtcNow + TimeSpan.FromSeconds(10))
            };
            badgeUpdater.Update(badgeNotification);
        }

        private void ShowShareUi(object sender, RoutedEventArgs e)
        {

            DataTransferManager.ShowShareUI();
        }

        private async void RunAppServiceTask(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<AppInfo> readOnlyList = await AppServiceCatalog.FindAppServiceProvidersAsync("AppServiceServer_kj4sv7dv9awfe");

            var appServiceConnection = new AppServiceConnection();

            appServiceConnection.AppServiceName = "AppServiceServerBackgroundTaskName";
            appServiceConnection.PackageFamilyName = "AppServiceServer_kj4sv7dv9awfe";

            AppServiceConnectionStatus status = await appServiceConnection.OpenAsync();
            if (status == AppServiceConnectionStatus.Success)
            {
                AppServiceResponse appServiceResponse = await appServiceConnection.SendMessageAsync(new ValueSet {{"a", null}, {"b", null}});
                ValueSet responseValueSet = appServiceResponse.Message;
                if (responseValueSet == null)
                {
                    RunAppServiceTextBlock.Text = "response is null";
                    return;
                }
                string response = responseValueSet.ContainsKey("count")
                    ? responseValueSet["count"].ToString()
                    : "missing response key";
                RunAppServiceTextBlock.Text = response;
                return;
            }
            
            RunAppServiceTextBlock.Text = "Failed to connect";
            return;
        }
    }
}
