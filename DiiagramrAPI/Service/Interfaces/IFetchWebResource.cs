using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IFetchWebResource
    {
        Task<string> DownloadStringAsync(string url);

        Task DownloadFileAsync(string url, string downloadToPath);
    }
}
