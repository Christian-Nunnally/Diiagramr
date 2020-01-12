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
            try
            {
                return Directory.Exists(path);
            }
            catch (Exception)
            {
                // TODO: catch more specific exception.
                throw;
            }
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
            catch (Exception)
            {
                // TODO: Handle specific exception
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
            catch (Exception)
            {
                return new List<string>();

                // TODO: Handle specific exception
                throw;
            }
        }

        public IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern).ToList();
            }
            catch (Exception)
            {
                return new List<string>();

                // TODO: Handle specific exception
                throw;
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