using Windows.UI;

namespace TestAppUWP.Samples.Map
{
    public class Customer
    {
        public Color Foreground;
        public Color Backround;
        public int Number;
        public bool Multi;
        public bool IsPhoneCall;
        public int VisitsCreateRecurringAppointments;
        public AppointmentEnums.AppointmentFlag AppointmentFlag;
        public double Latitude;
        public double Longitude;
    }

    public static class AppointmentEnums
    {
        public enum AppointmentTypeEnums
        {
            None = 0,
            Exception = 1,
            Recurrent = 2,
            OneShot = 3
        }

        public const int MinValueMultipleFlag = 100;

        public enum AppointmentFlag
        {
            None = 1,
            Fixed = 10,
            SelectedDay = 20,
            Today = 30,
            MultipleFixed = MinValueMultipleFlag,
            FixedAndSelectedDay = 200,
            FixedAndToday = 300,
            MultipleSelectedDay = 400,
            MultipleToday = 900
        }
    }

}
