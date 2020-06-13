using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using HttpClient = Windows.Web.Http.HttpClient;
using HttpMethod = Windows.Web.Http.HttpMethod;
using HttpRequestMessage = Windows.Web.Http.HttpRequestMessage;
using HttpResponseMessage = Windows.Web.Http.HttpResponseMessage;

namespace TestAppUWP.AppShell.Samples.Rsa
{
    public sealed partial class RsaPage
    {
        private readonly Uri _uri = new Uri("https://sfst.witglobal.net/");

        public RsaPage()
        {
            InitializeComponent();
        }

        private async void ButtonWindowsWebHttp_OnClick(object sender, RoutedEventArgs e)
        {
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
            List<HttpCookie> cookies = cookieManager.GetCookies(_uri).ToList();
            using (var httpClient = new HttpClient(httpBaseProtocolFilter))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, _uri);
                Request.Text = new StringBuilder().AppendLine(requestMessage.ToString())
                    .AppendLine(requestMessage.Headers.ToString())
                    .AppendLine(string.Join("\n", cookies)).ToString();

                HttpResponseMessage responseMessage = await httpClient.SendRequestAsync(requestMessage);

                Request1.Text = new StringBuilder().AppendLine(responseMessage.RequestMessage.ToString())
                    .AppendLine(string.Join(" - ", cookies)).ToString();

                Response.Text = responseMessage.ToString();
            }
        }

        private async void ButtonSystemNetHttp_OnClick(object sender, RoutedEventArgs e)
        {
            var httpClientHandler = new System.Net.Http.HttpClientHandler();
            
            CookieCollection cookieCollection = httpClientHandler.CookieContainer.GetCookies(_uri);
            var cookies = new Cookie[cookieCollection.Count];
            cookieCollection.CopyTo(cookies, 0);
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, _uri);
                Request.Text = new StringBuilder().AppendLine(requestMessage.ToString())
                    .AppendLine(requestMessage.Headers.ToString())
                    .AppendLine(string.Join("\n", cookies.ToList())).ToString();

                System.Net.Http.HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

                Request1.Text = new StringBuilder().AppendLine(responseMessage.RequestMessage.ToString())
                    .AppendLine(string.Join(" - ", cookies.ToList())).ToString();

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
