using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OCR.POC.Application.Repository;
using RestSharp;

namespace OCR.POC.Abbyy
{
    public class RestDownloader : IHttpDownloader
    {
        public async Task<Stream> DownloadAsync(string url, CancellationToken cancellationToken = new CancellationToken())
        {
            //TaskCompletionSource<Stream> taskCompletionSource = new TaskCompletionSource<Stream> ();
            //RestClient client = new RestClient(url);
            //RestRequest request = new RestRequest
            //{
            //    ResponseWriter = stream =>
            //    {
            //        taskCompletionSource.TrySetResult(stream);
            //    }
            //};

            //client.ExecuteTaskAsync(request);

            //return taskCompletionSource.Task;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "Get";
            WebResponse webResponse = await webRequest.GetResponseAsync();

            return webResponse.GetResponseStream();         
        }
    }
}
