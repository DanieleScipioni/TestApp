using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace TestAppUWP.Samples.Map
{
    public class BingMapsRestClient
    {
        private const string DistanceMatrixEndPoint = "https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?key={0}";

        private readonly string _bingMapsKey;

        public BingMapsRestClient(string bingMapsKey)
        {
            _bingMapsKey = bingMapsKey;
        }

        public async Task<object> DistanceMatrix(BasicGeoposition origin, IEnumerable<BasicGeoposition> destinations)
        {
            string json = JsonConvert.SerializeObject(new
            {
                origins = new[] {new {latitude = origin.Latitude, longitude = origin.Longitude}},
                destinations = destinations.Select(dest => new {latitude = dest.Latitude, longitude = dest.Longitude}),
                travelMode = "driving",
                timeUnit = "minute"
            });
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.PostAsync(new Uri(string.Format(DistanceMatrixEndPoint, _bingMapsKey), UriKind.Absolute),
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (responseMessage.IsSuccessStatusCode)
                {
                    return await responseMessage.Content.ReadAsStringAsync();
                }
                return null;
            }
        }
    }
}