using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TestAppUWP.Samples.Map
{
    public class RenderImageFlag : BaseRenderFlag
    {
        private readonly TextBlock _textBlock;
        private readonly Image _image;
        private readonly SemaphoreSlim _manualResetEvent;
        private bool _switch;

        public RenderImageFlag()
        {
            _manualResetEvent = new SemaphoreSlim(0,1);

            _image = new Image();
            _image.ImageOpened += (sender, args) => _manualResetEvent.Release();
            _image.ImageFailed += (sender, args) => _manualResetEvent.Release();

            _textBlock = new TextBlock {FontSize = 11, FontWeight = FontWeights.Bold, CharacterSpacing = -50};

            Children.Add(_image);
            Children.Add(_textBlock);

            Width = Height = 0;

            _switch = false;
        }

        protected override async Task GetRandomAccessStreamReferenceOverride(Color background, Color foregroud,
            string text, bool multi, AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall,
            int visitsCreateRecurringAppointments)
        {
            string uriString =
                _switch
                    ? CalculateCustomerFlagIconString(appointmentFlag, isPhoneCall)
                    : CalculateCustomerFlagIconStringForMap(appointmentFlag, visitsCreateRecurringAppointments,
                        isPhoneCall);
            _image.Source = new BitmapImage(new Uri(uriString));

            await _manualResetEvent.WaitAsync();

            if (uriString.EndsWith("map_circle_mini.png"))
            {
                SetLeft(_textBlock, 13 - text.Length * 1.5); SetTop(_textBlock, 6);
            }
            else
            {
                SetLeft(_textBlock, 2.5); SetTop(_textBlock, 5);
            }
            _switch = !_switch;

            _textBlock.Foreground = new SolidColorBrush(foregroud);
            _textBlock.Text = text + text;
        }

        public static string CalculateCustomerFlagIconString(AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall)
        {
            var valueReturn = "flag_grey.png";

            valueReturn = GetUriFromFlag(appointmentFlag, isPhoneCall) ?? valueReturn;
            return ToFlagUri(valueReturn);
        }

        private static string ToFlagUri(string valueReturn)
        {
            return $"ms-appx:///Assets/flags/{valueReturn}";
        }

        public static string CalculateCustomerFlagIconStringForMap(AppointmentEnums.AppointmentFlag appointmentFlag, int visitsCreateRecurringAppointments, bool isPhoneCall)
        {
            var valueReturn = "map_circle_mini.png";
            if (visitsCreateRecurringAppointments == 0)
            {
                valueReturn = "flag_up_red_mini.png";
            }

            valueReturn = GetUriFromFlag(appointmentFlag, isPhoneCall) ?? valueReturn;
            return ToFlagUri(valueReturn);
        }

        private static string GetUriFromFlag(AppointmentEnums.AppointmentFlag appointmentFlag, bool isPhoneCall)
        {

            string phoneCall = isPhoneCall ? "_phone" : "";
            switch (appointmentFlag)
            {
                case AppointmentEnums.AppointmentFlag.Today:
                    return $"flag_blue_mini{phoneCall}.png";
                case AppointmentEnums.AppointmentFlag.Fixed:
                    return $"flag_red_mini{phoneCall}.png";
                case AppointmentEnums.AppointmentFlag.SelectedDay:
                    return $"flag_green_mini{phoneCall}.png";
                case AppointmentEnums.AppointmentFlag.MultipleFixed:
                    return "flag_multi_red_mini.png";
                case AppointmentEnums.AppointmentFlag.FixedAndSelectedDay:
                    return "flag_multi_red_green_mini.png";
                case AppointmentEnums.AppointmentFlag.FixedAndToday:
                    return "flag_multi_red_blue_mini.png";
                case AppointmentEnums.AppointmentFlag.MultipleSelectedDay:
                    return "flag_multi_green_mini.png";
                case AppointmentEnums.AppointmentFlag.MultipleToday:
                    return "flag_multi_blue_mini.png";
                default:
                    return null;
            }
        }
    }
}
