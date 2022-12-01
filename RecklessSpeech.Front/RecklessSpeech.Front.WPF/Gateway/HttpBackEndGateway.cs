using Newtonsoft.Json;
using RecklessSpeech.Front.WPF.Dtos;
using RecklessSpeech.Front.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RecklessSpeech.Front.WPF.Gateway
{
    public class HttpBackEndGateway
    {
        private readonly IBackEndGatewayAccess access;
        private const string ApiVersion = "v1";

        public HttpBackEndGateway(IBackEndGatewayAccess access)
        {
            this.access = access;

        }

        public async Task ImportSequencesFromCsvFile(string filePath)
        {
            using MultipartFormDataContent content = new();

            FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

            content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            const string url = @$"https://localhost:47973/api/{ApiVersion}/sequences"; //todo mettre dans des settings

            await access.PostAsync(url, content);
        }
        public async Task<IReadOnlyCollection<SequenceDto>> GetAllSequences()
        {
            const string url = @$"https://localhost:47973/api/{ApiVersion}/sequences";

            HttpResponseMessage responseMessage = await access.GetAsync(url);

            string contentString = await responseMessage.Content.ReadAsStringAsync();

            IReadOnlyCollection<SequenceSummaryPresentation> result =
                JsonConvert.DeserializeObject<IReadOnlyCollection<SequenceSummaryPresentation>>(contentString);

            return result.Select(presentation => new SequenceDto()
            {
                Id = presentation.Id,
                Word = presentation.Word,
                Explanation = presentation.Explanation
            })
                .ToList();
        }

        public async Task<SequenceDto> GetOneSequence(Guid id)
        {
            string url = @$"https://localhost:47973/api/{ApiVersion}/sequences/{id}";

            HttpResponseMessage? responseMessage = await access.GetAsync(url);

            string contentString = await responseMessage.Content.ReadAsStringAsync();

            SequenceSummaryPresentation result =
                JsonConvert.DeserializeObject<SequenceSummaryPresentation>(contentString);

            return new()
            {
                Id = result.Id,
                Word = result.Word,
                Explanation = result.Explanation
            };
        }

        public async Task EnrichSequence(Guid id)
        {
            const string url = @$"https://localhost:47973/api/{ApiVersion}/sequences/Dictionary";

            HttpRequestMessage request = BuildJsonMessage(HttpMethod.Post, url, new List<Guid>() { id });

            await access.SendAsync(request);
        }

        public async Task SendSequenceToAnki(Guid id)
        {
            const string url = @$"https://localhost:47973/api/{ApiVersion}/sequences/Anki";

            HttpRequestMessage request = BuildJsonMessage(HttpMethod.Post, url, new List<Guid>() { id });

            await access.SendAsync(request);
        }

        public async Task<List<DictionaryDto>> GetAllDictionaries()
        {
            string url = @$"https://localhost:47973/api/{ApiVersion}/sequences/Dictionary";

            HttpResponseMessage? responseMessage = await access.GetAsync(url);

            List<DictionaryDto>? dictionaries = await responseMessage.Content.ReadFromJsonAsync<List<DictionaryDto>>();

            return dictionaries!;

        }

        private static HttpRequestMessage BuildJsonMessage(HttpMethod method, string path, object? parameters)
        {
            HttpRequestMessage request = new(method, new Uri(path, UriKind.RelativeOrAbsolute));
            if (parameters != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(parameters),
                    Encoding.UTF8,
                    "application/json");
            }

            return request;
        }

        public async Task<SequenceDto?> TryEnrichSequence(Guid sequenceDtoId, Guid dtoDictionaryId)
        {
            const string url = @$"https://localhost:47973/api/{ApiVersion}/sequences/Dictionary";

            EnrichSequenceParameter parameter = new(sequenceDtoId, dtoDictionaryId);
            HttpRequestMessage request = BuildJsonMessage(HttpMethod.Post, url, parameter);

            HttpResponseMessage response = await access.SendAsync(request);

            if (response.IsSuccessStatusCode is false) return null;

            SequenceDto? content = await response.Content.ReadFromJsonAsync<SequenceDto>();

            return content;
        }
    }

    public record EnrichSequenceParameter(Guid SequenceId, Guid DictionaryId);
}
