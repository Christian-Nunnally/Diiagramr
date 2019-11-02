using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IFetchWebResource : IService
    {
        Task DownloadFileAsync(string url, string downloadToPath);

        Task<string> DownloadStringAsync(string url);
    }
}