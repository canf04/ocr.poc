using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailGun;
using OCR.POC.Abbyy;
using OCR.POC.Application;
using OCR.POC.Application.Repository;
using RestSharp;
using RestSharp.Authenticators;

namespace Entry
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1.MailGun, x.Abbyy");

            if (Convert.ToInt32(Console.ReadLine()) == 1)
            {
                RunMailGun();

                return;
            }

            Console.WriteLine("Task watch duration ms:");
            CancellationTokenSource taskWatchSource = new CancellationTokenSource(int.Parse(Console.ReadLine()));
            CancellationToken taskWatchSourceToken = taskWatchSource.Token;
            IOcrRepository repository = new CloudApiClient(new RestClient("http://cloud.ocrsdk.com")
            {
                Authenticator = new HttpBasicAuthenticator("NTOcrPOC", "Te1gGa9uzTzRmU0FARDIr0Ez")
            });

            IHttpDownloader downloader = new RestDownloader();

            List<string> queuedTasks = new List<string>();
            object objLock = new object();

            Task.Run(async () =>
            {
                while (!taskWatchSourceToken.IsCancellationRequested)
                {
                    if (queuedTasks.Count > 0)
                    {
                        IReadOnlyCollection<TaskInfo> finishedTasks = await repository.GetFinishedTaskIdsAsync();

                        foreach (TaskInfo finishedTask in finishedTasks)
                        {
                            string id = queuedTasks.FirstOrDefault(i => i == finishedTask.Id);

                            if (!string.IsNullOrEmpty(id))
                            {
                                Console.WriteLine($"Text extraction completed for id: {finishedTask.Id}. Download link: {finishedTask.Url}");

                                using (FileStream sourceFileStream = File.Create($@"C:\Users\nenad.ilic\Desktop\abbyy\{finishedTask.Id}"))
                                {
                                    await (await downloader.DownloadAsync(finishedTask.Url)).CopyToAsync(sourceFileStream);

                                    Console.WriteLine($"File {sourceFileStream.Name} saved to disk.");
                                }
                            }

                            lock (objLock)
                            {
                                queuedTasks.Remove(id);
                            }
                        }
                    }

                    await Task.Delay(5000);
                }
            }).ContinueWith(t =>
            {
                var exception = t.Exception;
                if (exception != null)
                {
                    throw new Exception("Error during tasks checkup", exception);
                }
            });

            Console.WriteLine("Enter file name for ocr");
            string fileName = Console.ReadLine();

            FileStream imageStream = File.OpenRead(@"C:\Users\nenad.ilic\Desktop\" + fileName);
            try
            {
                repository.ProcessImageAsync(imageStream, CancellationToken.None).ContinueWith(t =>
                {
                    Console.WriteLine($"Text extraction queued with id: {t.Result}");
                    lock (objLock)
                    {
                        queuedTasks.Add(t.Result);
                    }
                });
            }
            catch (Exception)
            {

                imageStream.Dispose();
            }


            Console.ReadLine();
        }

        private static void RunMailGun()
        {
            var _config = new MailGunConfiguration();
            var docId = Guid.NewGuid();

            RestRequestBuilder request = RestRequestBuilder.Init(_config.BaseUrl)
                .UseBasicAuth(_config.ApiUsername, _config.ApiKey)
                .UsePostHttpMethod()
                .UseResourceUrl(_config.ResourceUrl)
                .WithParameterAsUrlSegment("domain", _config.Domain)
                .WithParameter("from", _config.From)
                .WithParameter("to", "qaudocx365@gmail.com")
                //.WithParameter("subject", _config.Subject)
                .WithParameter("subject", _config.Subject)
                .WithParameter("text", _config.Text)
                .WithParameter("v:id", docId)
                .WithParameter("v:anotherId", docId);


            IRestResponse response = request.Execute();

            RestRequestBuilder statsRequest = RestRequestBuilder.Init(_config.BaseUrl)
                .UseBasicAuth(_config.ApiUsername, _config.ApiKey)
                .UseGetHttpMethod()
                .UseResourceUrl(_config.StatsUrl)
                .WithParameter("begin", DateTime.UtcNow.AddMinutes(-60).ToString("r"))
                .WithParameter("end", DateTime.UtcNow.ToString("R"))
                .WithParameter("event", "accepted")
                .WithParameter("recipient", "qaudocx365@gmail.com")
                .WithParameterAsUrlSegment("domain", _config.Domain);

            StatsResponse eventsResponse = statsRequest.Execute<StatsResponse>();
            var eventsResponse2 = statsRequest.Execute();

        }
    }

    public class StatsResponse
    {
        public List<MailEvent> Items { get; set; }
    }

    public class MailEvent
    {
        public string Event { get; set; }
        public Message Message { get; set; }
        public UserVariables UserVariables { get; set; }
    }

    public class UserVariables
    {
        public string FileId { get; set; }
        public string AnotherId { get; set; }
    }

    public class Message
    {
        public List<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
    }
}
