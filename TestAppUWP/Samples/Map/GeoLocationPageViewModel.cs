using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using TestAppUWP.Core;
using Windows.ApplicationModel.Core;
using Windows.Services.Maps;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;

namespace TestAppUWP.Samples.Map
{
    public class GeoLocationPageViewModel : BindableBase, ICommand
    {
        private readonly MapControl _sessionMapControl;

        public GeoLocationPageViewModel()
        {
            try
            {
                if (_sessionMapControl == null)
                {
                    _sessionMapControl = new MapControl {MapServiceToken = MapServiceSettings.SelectedToken};
                }
            }
            catch
            {
                // ignore
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            ThreadPoolTimer.CreatePeriodicTimer(timer =>
            {
                var stopwatchElapsed = stopwatch.Elapsed;
                var _ = CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { Timer = stopwatchElapsed.ToString(); });
            }, TimeSpan.FromSeconds(1));
        }

        #region Properties

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                if (!SetProperty(ref _address, value)) return;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _location;
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private string _timer;
        public string Timer
        {
            get => _timer;
            set => SetProperty(ref _timer, value);
        }

        #endregion

        private async Task FindLocation()
        {
            var mapLocationFinderResult = await MapLocationFinder.FindLocationsAsync(_address, null, 1);
            if (mapLocationFinderResult.Status == MapLocationFinderStatus.Success)
            {
                var locations = mapLocationFinderResult.Locations;
                if (locations.Count > 0)
                {
                    var geopoint = locations[0].Point;
                    var geopointPosition = geopoint.Position;
                    Location = $"{geopointPosition.Latitude} {geopointPosition.Longitude}";
                }
                else
                {
                    Location = string.Empty;
                }
            }
            else
            {
                Location = mapLocationFinderResult.Status.ToString();
            }
        }

        #region ICommand

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(Address);
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                await FindLocation();
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}