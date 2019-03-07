using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

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
            RoutePlanMapControl.Children.Add(_customerUserControl);

            DataContextChanged += async (sender, args) =>
            {
                if (_viewModel == args.NewValue) return;

                if (_viewModel != null) _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;

                _viewModel = (MapViewModel) args.NewValue;
                if (_viewModel != null)
                {
                    try
                    {
                        RoutePlanMapControl.MapServiceToken = _viewModel.MapServiceToken;
                    }
                    catch
                    {
                        // ignore
                    }
                    await AddMapIcons();
                    _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
                }

                Bindings.Update();
            };
        }

        private async void ViewModelOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(MapViewModel.Customers)) return;
            await AddMapIcons();
        }

        private async void Cliccche2(object sender, RoutedEventArgs e)
        {
            _customerUserControl.Visibility = Visibility.Collapsed;
            RoutePlanMapControl.MapElements.Clear();
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
                RoutePlanMapControl.MapElements.Add(mapIcon);
            }

            GeoboundingBox geoboundingBox = GeoboundingBox.TryCompute(
                from MapElement m in RoutePlanMapControl.MapElements
                where m is MapIcon
                select ((MapIcon) m).Location.Position);
            await RoutePlanMapControl.TrySetViewBoundsAsync(geoboundingBox, new Thickness(8), MapAnimationKind.Linear);
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
            if (!(RoutePlanMapControl.MapElements[indexOf] is MapIcon mapIcon)) return;

            ShowFlyout(mapIcon, clickedItem);

            await RoutePlanMapControl.TrySetViewAsync(mapIcon.Location);
        }
    }
}
