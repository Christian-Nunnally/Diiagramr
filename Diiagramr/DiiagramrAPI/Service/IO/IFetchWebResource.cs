using System.Threading.Tasks;

namespace DiiagramrAPI.Service.IO
{
    /// <summary>
    /// Interface for downloading resources from the web.
    /// </summary>
    public interface IFetchWebResource : ISingletonService
    {
        /// <summary>
        /// Downloads a file from the web.
        /// </summary>
        /// <param name="url">The url of the page to download.</param>
        /// <param name="downloadToPath">The path to write the downloaded file to.</param>
        /// <returns>A task representing this work.</returns>
        Task DownloadFileAsync(string url, string downloadToPath);

        /// <summary>
        /// Download a web page and return it as a string.
        /// </summary>
        /// <param name="url">The url to the page to download.</param>
        /// <returns>The string contents of the web page.</returns>
        Task<string> DownloadStringAsync(string url);
    }
}