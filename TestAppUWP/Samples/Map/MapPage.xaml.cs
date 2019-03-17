using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

namespace TestAppUWP.Samples.Map
{
    public partial class MapPage
    {
        private MapViewModel _viewModel;
        private static CustomerUserControl _customerUserControl;
        private bool _mapIconClicked;

        public MapPage()
        {
            InitializeComponent();
            _customerUserControl = new CustomerUserControl {Visibility = Visibility.Collapsed};
            MapControl.SetNormalizedAnchorPoint(_customerUserControl, new Point(0, 1));
            MapControl.Children.Add(_customerUserControl);

            DataContextChanged += async (sender, args) =>
            {
                if (_viewModel == args.NewValue) return;

                if (_viewModel != null) _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;

                _viewModel = (MapViewModel) args.NewValue;
                if (_viewModel != null)
                {
                    try
                    {
                        MapControl.MapServiceToken = _viewModel.MapServiceToken;
                    }
                    catch {/*ignore*/}
                    await AddMapIcons();
                    _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
                }

                Bindings.Update();
            };
        }

        private async void ViewModelOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(MapViewModel.Customers):
                    await AddMapIcons();
                    break;
                case nameof(MapViewModel.MapRoute):
                    await AddMapRouteView(_viewModel.MapRoute);
                    break;
                case nameof(MapViewModel.MapServiceToken):
                    try
                    {
                        MapControl.MapServiceToken = _viewModel.MapServiceToken;
                    }
                    catch {/*ignore*/}
                    break;
            }
        }

        private async void ReloadCustomers(object sender, RoutedEventArgs e)
        {
            _customerUserControl.Visibility = Visibility.Collapsed;
            MapControl.MapElements.Clear();
            await _viewModel.ReloadCustmers();
        }

        private async Task AddMapIcons()
        {
            if (_viewModel.Customers == null) return;

            foreach (var customer in _viewModel.Customers)
            {
                var mapIcon = new MapIcon
                {
                    Image = await RenderImageFlag.GetRandomAccessStreamReference(customer.Backround, customer.Foreground,
                        customer.Number.ToString(), customer.Multi, customer.AppointmentFlag, customer.IsPhoneCall, customer.VisitsCreateRecurringAppointments),
                    Location = new Geopoint(new BasicGeoposition { Latitude = customer.Latitude, Longitude = customer.Longitude }),
                    NormalizedAnchorPoint = new Point(0.1, 0.8)
                };
                MapControl.MapElements.Add(mapIcon);
            }

            GeoboundingBox geoboundingBox = GeoboundingBox.TryCompute(
                from MapElement m in MapControl.MapElements
                where m is MapIcon
                select ((MapIcon) m).Location.Position);
            await MapControl.TrySetViewBoundsAsync(geoboundingBox, new Thickness(8), MapAnimationKind.Linear);
        }

        private async Task AddMapRouteView(MapRoute mapRoute)
        {
            var mapRouteView = new MapRouteView(mapRoute) {RouteColor = Colors.Red, OutlineColor = Colors.BlueViolet};
            MapControl.Routes.Clear();
            MapControl.Routes.Add(mapRouteView);
            await MapControl.TrySetViewBoundsAsync(mapRoute.BoundingBox, new Thickness(16), MapAnimationKind.Linear);
        }

        private void RoutePlanMapControl_OnMapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (!_mapIconClicked)
            {
                _customerUserControl.Visibility = Visibility.Collapsed;
            }
            _mapIconClicked = false;
        }

        private void RoutePlanMapControl_OnMapElementClick(MapControl mapControl, MapElementClickEventArgs args)
        {
            _mapIconClicked = true;

            List<MapElement> mapElements = args.MapElements.ToList();
            if (!(mapElements.FirstOrDefault() is MapIcon mapIcon)) return;

            int indexOf = mapControl.MapElements.IndexOf(mapIcon);
            if (indexOf == -1) return;

            Customer customer = _viewModel.Customers[indexOf];

            ShowFlyout(mapIcon, customer);

            ListView.SelectedIndex = indexOf;
            ListView.ScrollIntoView(customer);
        }

        private static void ShowFlyout(MapIcon mapIcon, Customer customer)
        {
            if (customer != _customerUserControl.DataContext)
            {
                _customerUserControl.Visibility = Visibility.Collapsed;

                _customerUserControl.DataContext = customer;
                MapControl.SetLocation(_customerUserControl, mapIcon.Location);
            }
            _customerUserControl.Visibility = Visibility.Visible;
        }

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Customer) e.ClickedItem;
            int indexOf = _viewModel.Customers.IndexOf(clickedItem);
            if (!(MapControl.MapElements[indexOf] is MapIcon mapIcon)) return;

            ShowFlyout(mapIcon, clickedItem);

            await MapControl.TrySetViewAsync(mapIcon.Location);
        }

        private async void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            e.Handled = true;

            var textBox = (TextBox) sender;
            MapLocationFinderResult mapLocationFinderResult = await _viewModel.FindLocation(textBox.Text);
            MapControl.MapElements.Clear();
            if (mapLocationFinderResult.Status != MapLocationFinderStatus.Success) return;
            if (mapLocationFinderResult.Locations.Count <= 0) return;
            MapLocation mapLocation = mapLocationFinderResult.Locations[0];
            var mapIcon = new MapIcon
            {
                Location = mapLocation.Point,
                Title = mapLocation.DisplayName,
            };
            MapControl.MapElements.Add(mapIcon);
            await MapControl.TrySetViewAsync(mapLocation.Point);
        }
    }
}
