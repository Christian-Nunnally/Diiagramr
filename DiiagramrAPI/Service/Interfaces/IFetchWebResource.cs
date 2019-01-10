using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IFetchWebResource : IDiiagramrService
    {
        Task<string> DownloadStringAsync(string url);

        Task DownloadFileAsync(string url, string downloadToPath);
    }
}
