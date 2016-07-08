using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            Console.WriteLine("Task watch duration ms:");
            CancellationTokenSource taskWatchSource = new CancellationTokenSource(int.Parse(Console.ReadLine()));
            CancellationToken taskWatchSourceToken = taskWatchSource.Token;
            IOcrRepository repository = new CloudApiClient(new RestClient("http://cloud.ocrsdk.com")
            {
                Authenticator = new HttpBasicAuthenticator("NTOcrPOC", "fVdG+KODhR2Oiv8BfcpEGYM+")
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
            
            using (FileStream imageStream = File.OpenRead(@"C:\Users\nenad.ilic\Desktop\" + fileName))
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

            Console.ReadLine();
        }
    }
}
