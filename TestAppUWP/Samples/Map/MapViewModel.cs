using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestAppUWP.Core;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;

namespace TestAppUWP.Samples.Map
{
    public class MapViewModel : BindableBase, ICommand
    {
        private readonly Random _random;

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private MapRoute _mapRoute;
        public MapRoute MapRoute
        {
            get => _mapRoute;
            set => SetProperty(ref _mapRoute, value);
        }

        private string _mapServiceToken;
        public string MapServiceToken
        {
            get => _mapServiceToken;
            set
            {
                if (SetProperty(ref _mapServiceToken, value))
                {
                    MapServiceSettings.SelectedToken = value;
                }
            }
        }

        public List<Tuple<string, string>> BingMapsKeys = new List<Tuple<string, string>>
        {
            new Tuple<string, string>(nameof(MapServiceSettings.TokenUwp1), MapServiceSettings.TokenUwp1),
            new Tuple<string, string>(nameof(MapServiceSettings.TokenUwp2), MapServiceSettings.TokenUwp2)
        };

        private bool _commandsEnabled;
        public bool CommandsEnabled
        {
            get => _commandsEnabled;
            set => SetProperty(ref _commandsEnabled, value);
        }

        public MapViewModel()
        {
            _mapServiceToken = MapServiceSettings.SelectedToken;
            _commandsEnabled = true;
            _random = new Random();
            Init();
        }

        private async void Init()
        {
            await ReloadCustmers();
        }

        private static Color GetColor(int index)
        {
            switch (index)
            {
                case 0:
                    return Colors.Green;
                case 1:
                    return Colors.Blue;
                case 2:
                    return Colors.Gray;
                case 3:
                    return Colors.LightSlateGray;
                case 4:
                    return Colors.LightSalmon;
                case 5:
                    return Colors.LightSkyBlue;
                case 6:
                    return Colors.DarkCyan;
                case 7:
                    return Colors.LightGreen;
                default:
                    return Colors.PaleVioletRed;
            }
        }

        private static Color GetForegroudColor(int index)
        {
            switch (index)
            {
                case 0:
                    return Colors.Black;
                default:
                    return Colors.White;
            }
        }

        public async Task ReloadCustmers()
        {
            await Task.Yield();

            var customers = new List<Customer>(200);

            var values = Enum.GetValues(typeof(AppointmentEnums.AppointmentFlag));
            
            var random = new Random(DateTime.UtcNow.Millisecond);
            for (var idx = 0; idx < 10; idx++)
            {
                customers.Add(new Customer
                {
                    Foreground = GetForegroudColor(random.Next(2)),
                    Backround = GetColor(random.Next(8)),
                    Number = random.Next(8) + 1,
                    Multi = random.Next(2) == 1,
                    IsPhoneCall = random.Next(2) == 1,
                    VisitsCreateRecurringAppointments = random.Next(2),
                    AppointmentFlag = (AppointmentEnums.AppointmentFlag)values.GetValue(random.Next(values.Length)),
                    Latitude = 41.891345957404056 + (random.NextDouble() - 0.5) / 10,
                    Longitude = 12.493643416971947 + (random.NextDouble() - 0.5) / 10

                });
            }
            
            Customers = new ObservableCollection<Customer>(customers);
        }

        private async Task ExecuteDistanceMatrix()
        {
            var geopositions = new BasicGeoposition[_customers.Count];
            for (var index = 0; index < _customers.Count; index++)
            {
                var customer = _customers[index];
                var geoposition = new BasicGeoposition { Latitude = customer.Latitude, Longitude = customer.Longitude };
                geopositions[index] = geoposition;
            }

            if (_customers.Count <= 1) return;

            var _ = await new BingMapsRestClient(MapServiceToken).DistanceMatrix(geopositions[0],
                geopositions.Skip(1));
        }

        private async Task ExecuteRoute()
        {
            var geopositions = new Geopoint[_customers.Count];
            for (var index = 0; index < _customers.Count; index++)
            {
                var customer = _customers[index];
                var geoposition = new Geopoint(new BasicGeoposition { Latitude = customer.Latitude, Longitude = customer.Longitude });
                geopositions[index] = geoposition;
            }

            MapRouteFinderResult mapRouteFinderResult = await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(
                geopositions, MapRouteOptimization.Time, MapRouteRestrictions.None);
            MapRoute = mapRouteFinderResult.Status == MapRouteFinderStatus.Success ? mapRouteFinderResult.Route : null;
        }

        public async Task<MapLocationFinderResult> FindLocation(string searchText)
        {
            return await MapLocationFinder.FindLocationsAsync(searchText, null, 1);
        }

        private async Task UseTransactions()
        {
            CommandsEnabled = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            for (var mapKeyIndex = 0; mapKeyIndex < 100; mapKeyIndex++)
            {
                MapServiceToken = BingMapsKeys[mapKeyIndex % BingMapsKeys.Count].Item2;

                int iterations = _random.Next(10);
                for (var index = 0; index < iterations; index++)
                {
                    await ReloadCustmers();
                    await ExecuteRoute();
                    await ExecuteDistanceMatrix();
                    await Task.Delay(2000);
                }
            }
            CommandsEnabled = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        public const string DistanceMatrixCommandParameter = "DistanceMatrix";
        public const string RouteCommandParameter = "Route";
        public const string TransactionsCommandParameter = "Transactions";

        public bool CanExecute(object parameter)
        {
            return _commandsEnabled;
        }

        public async void Execute(object parameter)
        {
            switch (parameter)
            {
                case string strinValue:
                {
                    switch (strinValue)
                    {
                        case DistanceMatrixCommandParameter:
                            await ExecuteDistanceMatrix();
                            break;
                        case RouteCommandParameter:
                            await ExecuteRoute();
                            break;
                        case TransactionsCommandParameter:
                            await UseTransactions();
                            break;
                    }
                    break;
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}