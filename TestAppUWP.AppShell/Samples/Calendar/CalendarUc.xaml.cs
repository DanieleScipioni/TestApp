using System;
using Windows.ApplicationModel.Appointments;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.AppShell.Samples.Calendar
{
    public sealed partial class CalendarUc
    {
        private readonly AppointmentCalendarOtherAppWriteAccess[] _appointmentStoreAccessType =
            (AppointmentCalendarOtherAppWriteAccess[])Enum.GetValues(typeof(AppointmentCalendarOtherAppWriteAccess));
        private AppointmentCalendarAdapter _appointmentCalendarAdapter;
        private AppointmentCalendar _appointmentCalendar;

        public CalendarUc()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is AppointmentCalendarAdapter appointmentCalendarAdapter &&
                    appointmentCalendarAdapter != _appointmentCalendarAdapter)
                {
                    _appointmentCalendarAdapter = appointmentCalendarAdapter;
                    _appointmentCalendar = _appointmentCalendarAdapter.AppointmentCalendar;
                    Bindings.Update();
                }
            };
        }

        public event EventHandler<AppointmentCalendarAdapter> DeleteClick;

        public event EventHandler<AppointmentCalendarAdapter> ErrorClick;

        private void ErrorButton_OnClick(object sender, RoutedEventArgs e)
        {
            ErrorClick?.Invoke(this, _appointmentCalendarAdapter);
        }

        private async void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (await new ContentDialog
            {
                Title = "Confirm",
                Content = "Are you sure?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Close
            }.ShowAsync() != ContentDialogResult.Primary) return;
            DeleteClick?.Invoke(this, _appointmentCalendarAdapter);
        }

        private void OtherAppWriteAccess_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null)
            {
                // ReSharper disable once PossibleNullReferenceException
                _appointmentCalendarAdapter.OtherAppWriteAccess =
                    (AppointmentCalendarOtherAppWriteAccess)comboBox.SelectedItem;
            }
        }

        private async void RegisterSyncManagerAsync_OnClick(object sender, RoutedEventArgs e)
        {
            await _appointmentCalendarAdapter.RegisterSyncManagerAsync();
        }
    }
}
