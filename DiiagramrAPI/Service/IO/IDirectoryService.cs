using System.Collections.Generic;
using System.IO;

namespace DiiagramrAPI.Service.IO
{
    /// <summary>
    /// interface for interacting with a directory based file system.
    /// </summary>
    public interface IDirectoryService : ISingletonService
    {
        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="path">The path to create the directory at.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="path">The path to the directory to delete.</param>
        /// <param name="recursive">Whether to recursively delete.</param>
        void Delete(string path, bool recursive);

        /// <summary>
        /// Gets whether a directory exists.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>True if the directory exists.</returns>
        bool Exists(string path);

        /// <summary>
        /// Extracts a .zip directory at the source path to the <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="sourcePath">The path to a zip directory.</param>
        /// <param name="destinationPath">The path to copy the extracted contents to.</param>
        void ExtractToDirectory(string sourcePath, string destinationPath);

        /// <summary>
        /// Get the current working directory.
        /// </summary>
        /// <returns>The current working directory.</returns>
        string GetCurrentDirectory();

        /// <summary>
        /// Get the names of all directories in the given path.
        /// </summary>
        /// <param name="path">The path to search</param>
        /// <returns>The names of all directories in <paramref name="path"/>.</returns>
        IEnumerable<string> GetDirectories(string path);

        /// <summary>
        /// Gets the directory name from a path.
        /// </summary>
        /// <param name="path">The path to extract the directory name from.</param>
        /// <returns>The extracted directory name.</returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Gets all of the file names in the given path.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The pattern to match files names to before returning them.</param>
        /// <returns>The names of the matched files.</returns>
        IEnumerable<string> GetFiles(string path, string searchPattern);

        /// <summary>
        /// Gets all of the file names in the given path.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The pattern to match files names to before returning them.</param>
        /// <param name="searchOption">Strategy to search using.</param>
        /// <returns>The names of the matched files.</returns>
        IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption);

        /// <summary>
        /// Move a file.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toPath">To path.</param>
        void Move(string fromPath, string toPath);

        /// <summary>
        /// Read all text from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>All the text in the file.</returns>
        string ReadAllText(string path);
    }
}