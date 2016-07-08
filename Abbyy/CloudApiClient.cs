using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OCR.POC.Application;
using OCR.POC.Application.Repository;
using RestSharp;

namespace OCR.POC.Abbyy
{
    public class CloudApiClient : IOcrRepository
    {
        private readonly RestClient _client;

        public CloudApiClient(RestClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
        }

        public async Task<string> ProcessImageAsync(Stream imageStream, CancellationToken cancellationToken)
        {
            RestRequest request = new RestRequest("processImage") { AlwaysMultipartFormData = true };
            request.AddQueryParameter("profile", "textExtraction");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);
                request.AddFileBytes("test", memoryStream.ToArray(), "test");
            }

            IRestResponse<TaskResponse> response = await _client.ExecutePostTaskAsync<TaskResponse>(request, cancellationToken);

            return response.Data.id;
        }

        public async Task<IReadOnlyCollection<TaskInfo>> GetFinishedTaskIdsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            RestRequest request = new RestRequest("listFinishedTasks");

            IRestResponse<Response> response = await _client.ExecuteGetTaskAsync<Response>(request, cancellationToken);

            return response.Data.Value.Select(t => new TaskInfo { Id = t.id, Url = t.resultUrl }).ToArray();
        }
    }

    internal class Response
    {
        public List<TaskResponse> Value { get; set; }
    }
}

