using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TestAppUWP.Core;
using Windows.UI;

namespace TestAppUWP.Samples.Map
{
    public class MapViewModel : BindableBase
    {
        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public MapViewModel()
        {
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
                    return Colors.LawnGreen;
                case 1:
                    return Colors.LightBlue;
                case 2:
                    return Colors.LightCoral;
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

            var random = new Random(DateTime.UtcNow.Millisecond);
            for (var idx = 0; idx < 200; idx++)
            {
                customers.Add(new Customer
                {
                    Foreground = GetForegroudColor(random.Next(2)),
                    Backround = GetColor(random.Next(8)),
                    Number = random.Next(8) + 1,
                    Multi = random.Next(2) == 1,
                    Latitude = 41.65 + random.NextDouble() / 2,
                    Longitude = 12.05 + random.NextDouble()
                });
            }
            
            Customers = new ObservableCollection<Customer>(customers);
        }
    }
}