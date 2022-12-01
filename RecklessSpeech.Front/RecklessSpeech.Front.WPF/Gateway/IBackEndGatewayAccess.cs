using System.Net.Http;
using System.Threading.Tasks;

namespace RecklessSpeech.Front.WPF.Gateway
{
    public interface IBackEndGatewayAccess
    {
        Task PostAsync(string url, MultipartFormDataContent content);
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
