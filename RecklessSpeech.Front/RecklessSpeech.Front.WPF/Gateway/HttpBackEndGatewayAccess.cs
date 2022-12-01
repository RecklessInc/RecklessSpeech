using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RecklessSpeech.Front.WPF.Gateway
{
    public class HttpBackEndGatewayAccess : IBackEndGatewayAccess
    {
        public async Task PostAsync(string url, MultipartFormDataContent content)
        {
            using HttpClient client = new();

            await client.PostAsync(new Uri(url), content);
        }
        public async Task<HttpResponseMessage> GetAsync(string url)
        {

            using HttpClient client = new();

            return await client.GetAsync(new Uri(url));
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            HttpClient client = new();
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
    }
}
