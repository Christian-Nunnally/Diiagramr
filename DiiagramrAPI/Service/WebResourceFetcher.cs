using System;
using System.Net;
using System.Threading.Tasks;
using DiiagramrAPI.Service.Interfaces;

namespace DiiagramrAPI.Service
{
    public class WebResourceFetcher : IFetchWebResource
    {
        private readonly WebClient _webClient = new WebClient();

        public async Task<string> DownloadStringAsync(string url)
        {
            try
            {
                return await Task.Run(() => _webClient.DownloadString(url));
            }
            catch
            {
                return null;
            }
        }

        public async Task DownloadFileAsync(string url, string downloadToPath)
        {
            try
            {
                await Task.Run(() => _webClient.DownloadFile(new Uri(url), downloadToPath));
            }
            catch
            {
            }
        }
    }
}