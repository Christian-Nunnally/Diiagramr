using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IFetchWebResource : IDiiagramrService
    {
        Task DownloadFileAsync(string url, string downloadToPath);

        Task<string> DownloadStringAsync(string url);
    }
}
