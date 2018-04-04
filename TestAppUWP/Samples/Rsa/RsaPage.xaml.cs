using System;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace TestAppUWP.Samples.Rsa
{
    public sealed partial class RsaPage
    {
        public RsaPage()
        {
            InitializeComponent();
        }

        private async void ButtonCall_OnClick(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://sfst.witglobal.net/");
            var httpBaseProtocolFilter = new HttpBaseProtocolFilter
            {
                AllowUI = true,
                CacheControl =
                {
                    ReadBehavior = HttpCacheReadBehavior.NoCache,
                    WriteBehavior = HttpCacheWriteBehavior.NoCache
                }
            };
            HttpCookieManager cookieManager = httpBaseProtocolFilter.CookieManager;
            var cookies = cookieManager.GetCookies(uri).ToList();
            using (var httpClient = new HttpClient(httpBaseProtocolFilter))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                    uri);
                Request.Text = new StringBuilder().AppendLine(requestMessage.ToString())
                    .AppendLine(requestMessage.Headers.ToString())
                    .AppendLine(string.Join("\n", cookies)).ToString();
                HttpResponseMessage responseMessage = await httpClient.SendRequestAsync(requestMessage);
                Request1.Text = new StringBuilder().AppendLine(responseMessage.RequestMessage.ToString())
                    .AppendLine(string.Join(" - ", cookies)).ToString();
                Response.Text = responseMessage.ToString();
            }
        }

        private void ButtonClearCookies_OnClick(object sender, RoutedEventArgs e)
        {
            HttpCookieManager cookieManager = new HttpBaseProtocolFilter().CookieManager;
            HttpCookie[] httpCookieCollection = cookieManager.GetCookies(new Uri("https://sfst.witglobal.net")).ToArray();
            foreach (HttpCookie cookie in httpCookieCollection)
            {
                cookieManager.DeleteCookie(cookie);
            }
        }
    }
}
