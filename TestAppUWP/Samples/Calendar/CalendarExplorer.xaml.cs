using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.ApplicationModel.Appointments;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.Samples.Calendar
{
    public sealed partial class CalendarExplorer
    {
        private AppointmentStore _appointmentStore;

        private ObservableCollection<AppointmentCalendarAdapter> _appointmentCalendarAdapters;
        private AppointmentCalendar _appointmentCalendar;
        private ObservableCollection<Appointment> _appointments;
        private readonly List<string> _appointmentStoreAccessType = Enum.GetNames(typeof(AppointmentStoreAccessType)).ToList();

        private int _selectedAppointmentStoreAccessTypeIndex;
        public int SelectedAppointmentStoreAccessTypeIndex
        {
            get => _selectedAppointmentStoreAccessTypeIndex;
            set
            {
                if (_selectedAppointmentStoreAccessTypeIndex == value) return;
                _selectedAppointmentStoreAccessTypeIndex = value;
                StoreAccessTypeChanged((AppointmentStoreAccessType) value);
            }
        }

        public CalendarExplorer()
        {
            InitializeComponent();

            Loaded += async (sender, args) =>
            {
                var selectedAppointmentStoreAccessTypeIndex = (AppointmentStoreAccessType)_selectedAppointmentStoreAccessTypeIndex;
                await RequestStore(selectedAppointmentStoreAccessTypeIndex);
                await LoadCalendars();
            };
        }

        private async void StoreAccessTypeChanged(AppointmentStoreAccessType appointmentStoreAccessType)
        {
            await RequestStore(appointmentStoreAccessType);
            await LoadCalendars();
        }

        private async Task RequestStore(AppointmentStoreAccessType appointmentStoreAccessType)
        {
            _appointmentStore = await AppointmentManager.RequestStoreAsync(appointmentStoreAccessType);
        }

        private async Task LoadCalendars()
        {
            IReadOnlyList<AppointmentCalendar> calendars =
                await _appointmentStore.FindAppointmentCalendarsAsync(FindAppointmentCalendarsOptions
                    .IncludeHidden);

            _appointmentCalendarAdapters =
                new ObservableCollection<AppointmentCalendarAdapter>(
                    (from calendar in calendars select new AppointmentCalendarAdapter(calendar)).ToList());
            CalendarListView.ItemsSource = _appointmentCalendarAdapters;
        }

        private async void DeleteButton_OnClick(object sender, AppointmentCalendarAdapter appointmentCalendarAdapter)
        {
            await DeleteAppointmentCalendar(appointmentCalendarAdapter);
        }

        internal async Task DeleteAppointmentCalendar(AppointmentCalendarAdapter appointmentCalendarAdapter)
        {
            if (await appointmentCalendarAdapter.DeleteAsync())
            {
                _appointmentCalendarAdapters.Remove(appointmentCalendarAdapter);
            }
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CalendarListView.Visibility == Visibility.Visible)
            {
                await LoadCalendars();
            }
            else
            {
                await LoadAppointments();
            }
        }

        private async void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CalendarListView.Visibility == Visibility.Visible)
            {
                await AddCalendar();
            }
            else
            {
                await AddAppointment();
            }
        }

        private async Task AddCalendar()
        {
            bool tryGetValue = Resources.TryGetValue("DataTemplate", out object datatemplateResource);
            if (!tryGetValue || !(datatemplateResource is DataTemplate newCalendarDatatemplate)) return;

            var calendarSourcesByDisplayName = new Dictionary<string, CalendarSource>();
            IReadOnlyList<AppointmentCalendar> readOnlyList = await _appointmentStore.FindAppointmentCalendarsAsync(FindAppointmentCalendarsOptions.IncludeHidden);
            foreach (AppointmentCalendar appointmentCalendar in readOnlyList)
            {
                if (calendarSourcesByDisplayName.ContainsKey(appointmentCalendar.SourceDisplayName)) continue;
                calendarSourcesByDisplayName[appointmentCalendar.SourceDisplayName] = new CalendarSource
                {
                    DisplayName = appointmentCalendar.SourceDisplayName,
                    UserDataAccountId = appointmentCalendar.UserDataAccountId
                };
            }

            var calendarSourceList = new CalendarSourceList {CalendarSources = calendarSourcesByDisplayName.Values.ToList() };
            var contentDialog = new ContentDialog
            {
                Title = "New Calendar",
                PrimaryButtonText = "Create",
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Primary,
                ContentTemplate = newCalendarDatatemplate,
                DataContext = calendarSourceList
            };
            ContentDialogResult contentDialogResult = await contentDialog.ShowAsync();
            if (contentDialogResult != ContentDialogResult.Primary) return;
            CalendarSource selectedCalendarSource = calendarSourceList.SelectedCalendarSource;
            if (!string.IsNullOrWhiteSpace(calendarSourceList.NewCalendarName))
            {
                try
                {
                    AppointmentCalendar appointmentCalendar;
                    if (selectedCalendarSource == null)
                    {
                        appointmentCalendar =
                            await _appointmentStore.CreateAppointmentCalendarAsync(calendarSourceList.NewCalendarName);
                    }
                    else
                    {
                        appointmentCalendar = await _appointmentStore.CreateAppointmentCalendarAsync(
                            calendarSourceList.NewCalendarName,
                            selectedCalendarSource.UserDataAccountId);
                    }
                    _appointmentCalendarAdapters.Insert(0, new AppointmentCalendarAdapter(appointmentCalendar));
                }
                catch (Exception)
                {
                    contentDialog = new ContentDialog
                    {
                        Title = "Error creating calendar",
                        Content = "Try to change AppointmentStoreAccessType",
                        CloseButtonText = "Close",
                        DefaultButton = ContentDialogButton.Close
                    };
                    await contentDialog.ShowAsync();
                }
            }
        }

        private async void ErrorButton_OnClick(object sender, AppointmentCalendarAdapter appointmentCalendarAdapter)
        {
            await ResetError(appointmentCalendarAdapter);
        }

        internal async Task ResetError(AppointmentCalendarAdapter appointmentCalendarAdapter)
        {
            AppointmentCalendar appointmentCalendar = appointmentCalendarAdapter.AppointmentCalendar;
            int indexOf = _appointmentCalendarAdapters.IndexOf(appointmentCalendarAdapter);
            AppointmentCalendar appointmentCalendarAsync = await _appointmentStore.GetAppointmentCalendarAsync(appointmentCalendar.LocalId);
            var calendarAdapter = new AppointmentCalendarAdapter(appointmentCalendarAsync);
            _appointmentCalendarAdapters.RemoveAt(indexOf);
            _appointmentCalendarAdapters.Insert(indexOf, calendarAdapter);
        }

        private async void ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var appointmentCalendarAdapter = (AppointmentCalendarAdapter) e.ClickedItem;
            CalendarListView.Visibility = Visibility.Collapsed;
            AppointmentSection.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
            _appointmentCalendar = appointmentCalendarAdapter.AppointmentCalendar;
            AppointmentListView.Header = _appointmentCalendar.DisplayName;
            await LoadAppointments();
        }

        private async Task LoadAppointments()
        {
            AppointmentListView.ItemsSource = _appointments = new ObservableCollection<Appointment>(
                await _appointmentCalendar.FindAppointmentsAsync(
                    DateTimeOffset.Now.Date.Add(TimeSpan.FromDays(-8)),
                    TimeSpan.FromDays(16),
                    new FindAppointmentsOptions
                    {
                        IncludeHidden = true
                    }));
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            CalendarListView.Visibility = Visibility.Visible;
            AppointmentSection.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            AppointmentListView.ItemsSource = null;
            _appointmentCalendar = null;
            AppointmentListView.Header = string.Empty;
        }

        private async Task AddAppointment()
        {
            var now = DateTimeOffset.Now;

            var appointment = new Appointment
            {
                Subject = "Appointment",
                StartTime = now.AddHours(-5),
                Duration = TimeSpan.FromMinutes(30),
            };

            await _appointmentCalendar.SaveAppointmentAsync(appointment);
            if (appointment.LocalId != string.Empty) _appointments.Add(appointment);

            var appointmentRecurrence = new AppointmentRecurrence {Unit = AppointmentRecurrenceUnit.Weekly, Interval = 1, DaysOfWeek = (AppointmentDaysOfWeek) (now.DayOfWeek + 1)};
            appointment = new Appointment
            {
                Subject = "Appointment with recurrence",
                StartTime = now.AddHours(1),
                Duration = TimeSpan.FromMinutes(30),
                Recurrence = appointmentRecurrence
            };
            
            await _appointmentCalendar.SaveAppointmentAsync(appointment);
            if (appointment.LocalId != string.Empty) _appointments.Add(appointment);
        }

        private async void DeleteAppointmentButton_OnClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement) sender;
            var appointment = (Appointment) frameworkElement.DataContext;
            await _appointmentCalendar.DeleteAppointmentAsync(appointment.LocalId);
            _appointments.Remove(appointment);
        }
    }

    internal class CalendarSourceList
    {
        public List<CalendarSource> CalendarSources;
        public CalendarSource SelectedCalendarSource;
        public string NewCalendarName;
    }
    
    internal class CalendarSource
    {
        public string UserDataAccountId { get; set; }
        public string DisplayName { get; set; }
    }

    public class AppointmentCalendarAdapter : BindableBase
    {
        public readonly AppointmentCalendar AppointmentCalendar;

        public AppointmentCalendarAdapter(AppointmentCalendar appointmentCalendar)
        {
            AppointmentCalendar = appointmentCalendar;
            _displayName = appointmentCalendar.DisplayName;
            _isHidden = appointmentCalendar.IsHidden;
            _otherAppWriteAccess = appointmentCalendar.OtherAppWriteAccess;
        }

        public string SourceDisplayName => AppointmentCalendar.SourceDisplayName;

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (!SetProperty(ref _displayName, value)) return;
                AppointmentCalendar.DisplayName = value;
                SaveCalendar();
            }
        }

        private AppointmentCalendarOtherAppWriteAccess _otherAppWriteAccess;
        public AppointmentCalendarOtherAppWriteAccess OtherAppWriteAccess
        {
            get => _otherAppWriteAccess;
            set
            {
                if (!SetProperty(ref _otherAppWriteAccess, value)) return;
                AppointmentCalendar.OtherAppWriteAccess = _otherAppWriteAccess;
                SaveCalendar();
            }
        }

        public AppointmentCalendarSyncManager SyncManager => AppointmentCalendar.SyncManager;

        public async Task RegisterSyncManagerAsync()
        {
            try
            {
                await AppointmentCalendar.RegisterSyncManagerAsync();
            }
            catch (Exception)
            {
                // ignore
            }
            OnPropertyChangedByName(nameof(SyncManager));
        }

        private async void SaveCalendar()
        {
            try
            {
                await AppointmentCalendar.SaveAsync();
                HasErrors = false;
            }
            catch (Exception)
            {
                HasErrors = true;
            }
            OnPropertyChangedByName(nameof(HasErrors));
        }

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set
            {
                if (!SetProperty(ref _isHidden, value)) return;
                AppointmentCalendar.IsHidden = value;
                SaveCalendar();
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                await AppointmentCalendar.DeleteAsync();
                HasErrors = false;
            }
            catch (Exception)
            {
                HasErrors = true;
            }
            OnPropertyChangedByName(nameof(HasErrors));
            return !HasErrors;
        }

        public bool HasErrors { get; private set; }
    }
}