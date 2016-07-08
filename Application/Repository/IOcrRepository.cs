using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OCR.POC.Application.Repository
{
    public interface IOcrRepository
    {
        Task<string> ProcessImageAsync(Stream imageStream, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<TaskInfo>> GetFinishedTaskIdsAsync(CancellationToken cancellationToken = default (CancellationToken));
    }
}
