using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  async Task<string> ProcessImageAsync(Stream imageStream, CancellationToken cancellationToken)
        {
            RestRequest request = new RestRequest("processImage") { Method = Method.POST };
            request.AddQueryParameter("exportFormat", "pdfa");
 
            request.Files.Add(new FileParameter
            {
                Name = "test",
                FileName = "test.pdf",
                ContentLength = imageStream.Length,
                ContentType = "application/pdf",
                Writer = s =>
                {
                    imageStream.CopyTo(s);
                }
            });


            IRestResponse<TaskResponse> response = await _client.ExecuteTaskAsync<TaskResponse>(request);

            //return Task.FromResult(response.Data.id);
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

