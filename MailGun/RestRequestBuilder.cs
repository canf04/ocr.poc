using System;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace MailGun
{
    public class RestRequestBuilder
    {
        private RestClient _client;
        private RestRequest _request;

        public static RestRequestBuilder Init(string baseUrl)
        {
            return new RestRequestBuilder
            {
                _client = new RestClient(baseUrl),
                _request = new RestRequest()
            };
        }

        public RestRequestBuilder UseBasicAuth(string username, string password)
        {
            _client.Authenticator = new HttpBasicAuthenticator(username, password);
            return this;
        }

        public RestRequestBuilder UsePostHttpMethod()
        {
            _request.Method = Method.POST;
            return this;
        }

        public RestRequestBuilder UseGetHttpMethod()
        {
            _request.Method = Method.GET;
            return this;
        }


        public RestRequestBuilder UseResourceUrl(string resource)
        {
            _request.Resource = resource;
            return this;
        }

        public RestRequestBuilder WithParameter(string name, object value)
        {
            _request.AddParameter(name, value);
            return this;
        }

        public RestRequestBuilder WithParameterAsUrlSegment(string name, string value)
        {
            _request.AddParameter(name, value, ParameterType.UrlSegment);
            return this;
        }

        public RestRequestBuilder AttachFile(byte[] bytes, string fileName)
        {
            _request.AddFile("attachment", bytes, fileName);
            return this;
        }

        public RestRequestBuilder AttachFile(Action<Stream> writer, string fileName)
        {
            _request.AddFile("attachment", writer, fileName);
            return this;
        }

        public IRestResponse Execute()
        {
            return _client.Execute(_request);
        }

        public T Execute<T>() where T : new()
        {
            return _client.Execute<T>(_request).Data;

        }

        public Task<IRestResponse> ExecuteAsync()
        {
            return _client.ExecuteTaskAsync(_request);
        }

        public async Task<T> ExecutAsync<T>(Func<IRestResponse, T> map)
        {
            var response = await _client.ExecuteTaskAsync(_request);
            return map(response);
        }
    }
}