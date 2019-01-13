using DiiagramrAPI.Service.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service
{
    public class WebResourceFetcher : IFetchWebResource
    {
        private readonly WebClient _webClient = new WebClient();
        private bool _currentlyFetching = false;

        public async Task<string> DownloadStringAsync(string url)
        {
            if (_currentlyFetching)
            {
                return string.Empty;
            }
            _currentlyFetching = true;

            try
            {
                _currentlyFetching = true;
                return await Task.Run(() => _webClient.DownloadString(url));
            }
            catch
            {
                return null;
            }
            finally
            {
                _currentlyFetching = false;
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