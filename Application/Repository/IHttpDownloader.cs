using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OCR.POC.Application.Repository
{
    public interface IHttpDownloader
    {
        Task<Stream> DownloadAsync(string url, CancellationToken cancellationToken = default(CancellationToken));
    }
}
