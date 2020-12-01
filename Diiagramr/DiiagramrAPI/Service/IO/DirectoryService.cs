using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DiiagramrAPI.Service.IO
{
    /// <summary>
    /// <see cref="IDirectoryService"/> implementation that wraps <see cref="Directory"/> to direct calls to the real file system.
    /// </summary>
    public class DirectoryService : IDirectoryService
    {
        /// <inheritdoc/>
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc/>
        public void Delete(string path, bool recursive)
        {
            if (path.Contains('.'))
            {
                File.Delete(path);
            }
            else
            {
                Directory.Delete(path, recursive);
            }
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public void ExtractToDirectory(string from, string to)
        {
            ZipFile.ExtractToDirectory(from, to);
        }

        /// <inheritdoc/>
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetDirectories(string path)
        {
            try
            {
                return Directory.GetDirectories(path).ToList();
            }
            catch (Exception e) when (
                e is ArgumentException
                || e is ArgumentNullException
                || e is ArgumentOutOfRangeException
                || e is UnauthorizedAccessException
                || e is PathTooLongException
                || e is IOException
                || e is DirectoryNotFoundException)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern, searchOption).ToList();
            }
            catch (Exception e) when (
                e is ArgumentException
                || e is ArgumentNullException
                || e is IOException
                || e is UnauthorizedAccessException
                || e is PathTooLongException
                || e is IOException
                || e is DirectoryNotFoundException)
            {
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern).ToList();
            }
            catch (Exception e) when (
                e is ArgumentException
                || e is ArgumentNullException
                || e is IOException
                || e is UnauthorizedAccessException
                || e is PathTooLongException
                || e is IOException
                || e is DirectoryNotFoundException)
            {
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public void Move(string fromPath, string toPath)
        {
            Directory.Move(fromPath, toPath);
        }

        /// <inheritdoc/>
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}