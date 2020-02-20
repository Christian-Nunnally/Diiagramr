﻿using System;
using System.Net;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.IO
{
    public sealed class WebResourceFetcher : IFetchWebResource, IDisposable
    {
        private readonly WebClient _webClient = new WebClient();
        private bool _currentlyFetching = false;

        public void Dispose()
        {
            _webClient.Dispose();
        }

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