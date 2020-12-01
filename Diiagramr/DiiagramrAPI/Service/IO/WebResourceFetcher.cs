using System;
using System.Net;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.IO
{
    /// <summary>
    /// Downloads web pages and scrapes the text off of them.
    /// </summary>
    public sealed class WebResourceFetcher : IFetchWebResource, IDisposable
    {
        private readonly WebClient _webClient = new WebClient();
        private bool _currentlyFetching = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            _webClient.Dispose();
        }

        /// <inheritdoc/>
        public async Task DownloadFileAsync(string url, string downloadToPath)
        {
            try
            {
                await Task.Run(() => _webClient.DownloadFile(new Uri(url), downloadToPath));
            }
            catch (Exception e) when (
                e is ArgumentNullException
                || e is NotSupportedException
                || e is WebException)
            {
                throw;
            }
        }

        /// <inheritdoc/>
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
            catch (Exception e) when (
                e is ArgumentNullException
                || e is NotSupportedException
                || e is WebException)
            {
                throw;
            }
            finally
            {
                _currentlyFetching = false;
            }
        }
    }
}