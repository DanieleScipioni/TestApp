﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAppBackgroundTask;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources.Core;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace TestAppUWP.Samples.CertTutorial
{
    public sealed partial class SamePage
    {
        private readonly ApplicationTrigger _applicationTrigger;

        public string Parameter { get; private set; }

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
            List<string> _ = resourceContext.QualifierValues.Keys.ToList();

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
                    backgroundTaskRegistration.Completed += (registration, eventArgs) =>
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

            CtorAsync();
        }

        private async void CtorAsync()
        {
            IReadOnlyList<SecondaryTile> readOnlyList = await SecondaryTile.FindAllAsync();
            foreach (SecondaryTile secondaryTile in readOnlyList)
            {
                // ReSharper disable once PossibleNullReferenceException
                SecondaryTileListBox.Items.Add(secondaryTile.TileId);
            }

            IReadOnlyList<StorageFolder> storageFolders = (await Package.Current.InstalledLocation.GetFoldersAsync()).ToList();

            ulong _ = ApplicationData.Current.RoamingStorageQuota;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string textBoxText = (string)e.Parameter ?? string.Empty;
            if (TextBlock != null) TextBlock.Text = textBoxText;
            if (TextBox != null) TextBox.Text = textBoxText;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Parameter = TextBox.Text;
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
            var valueSet = new ValueSet {{"CT", 1}};
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
            string tileId = GetTileId(TextBox.Text);
            bool exists = SecondaryTile.Exists(tileId);
            if (exists) return;

            var secondaryTile = new SecondaryTile(tileId)
            {
                DisplayName = $"Display {TextBox.Text}",
                Arguments = TextBox.Text
            };
            secondaryTile.VisualElements.Square150x150Logo =
                new Uri("ms-appx:///assets/Square150x150Logo.scale-200.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

            GeneralTransform buttonTransform = ((FrameworkElement)sender).TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            if (await secondaryTile.RequestCreateAsync(point))
            {
                // ReSharper disable once PossibleNullReferenceException
                SecondaryTileListBox.Items.Add(tileId);
            }
        }

        private string GetTileId(string text)
        {
            string trim = text.Trim();
            return trim == string.Empty ? string.Empty : $"{GetType().Name}_{text.Trim()}";
        }

        private async void SecondaryTileListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems.FirstOrDefault() is string tileId)) return;

            if (await new SecondaryTile(tileId).RequestDeleteAsync())
            {
                // ReSharper disable once PossibleNullReferenceException
                SecondaryTileListBox.Items.Remove(tileId);
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
            IReadOnlyList<AppInfo> readOnlyList =
                (await AppServiceCatalog.FindAppServiceProvidersAsync("AppServiceServer_kj4sv7dv9awfe")).ToList();
            foreach (AppInfo _ in readOnlyList) {}

            var appServiceConnection = new AppServiceConnection
            {
                AppServiceName = "AppServiceServerBackgroundTaskName",
                PackageFamilyName = "AppServiceServer_kj4sv7dv9awfe"
            };

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
        }

        private async void CameraCapture_OnClick(object sender, RoutedEventArgs e)
        {
            var cameraCaptureUi = new CameraCaptureUI();
            StorageFile photoFile = await cameraCaptureUi.CaptureFileAsync(CameraCaptureUIMode.Photo);

            using (IRandomAccessStreamWithContentType stream = await photoFile.OpenReadAsync())
            {
                BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await bitmapDecoder.GetSoftwareBitmapAsync();
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                var softwareBitmapSource = new SoftwareBitmapSource();
                await softwareBitmapSource.SetBitmapAsync(softwareBitmap);
                CameraCaptureImage.Source = softwareBitmapSource;
            }
        }

        private void ElementRect_OnClick(object sender, RoutedEventArgs e)
        {
            var uiElement = (UIElement) sender;
            Point transformPoint1 = Window.Current.Content.RenderTransform.TransformPoint(uiElement.RenderTransformOrigin);
            Point transformPoint = uiElement.RenderTransform.TransformPoint(new Point());
            Point point = uiElement.TransformToVisual(null).TransformPoint(new Point());
        }

        private async void EmailClick(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.List;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            fileOpenPicker.CommitButtonText = "?";
            fileOpenPicker.FileTypeFilter.Add("*");
            StorageFile storageFile = await fileOpenPicker.PickSingleFileAsync();
            RandomAccessStreamReference randomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(storageFile);
            var emailMessage = new EmailMessage
            {
                Subject = "my favorate app",
                Body = "I would like to tell you ..."
            };
            var emailAttachment = new EmailAttachment(storageFile.Name, randomAccessStreamReference);
            emailMessage.Attachments.Add(emailAttachment);
            var emailRecipient = new EmailRecipient("daniele.scipioni@gmail.com");
            emailMessage.To.Add(emailRecipient);
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
}
