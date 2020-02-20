using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DiiagramrAPI.Service.IO
{
    public class DirectoryService : IDirectoryService
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

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

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void ExtractToDirectory(string from, string to)
        {
            ZipFile.ExtractToDirectory(from, to);
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

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

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

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

        public void Move(string fromPath, string toPath)
        {
            Directory.Move(fromPath, toPath);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}