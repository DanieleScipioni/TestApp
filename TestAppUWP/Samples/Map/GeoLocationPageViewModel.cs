using System;
using Windows.Services.Maps;

namespace TestAppUWP.Samples.Map
{
    public class GeoLocationPageViewModel
    {
        public GeoLocationPageViewModel()
        {
            try
            {
                MapService.ServiceToken = MapServiceSettings.Token;
            }
            catch
            {
                // ignore
            }
        }
    }
}