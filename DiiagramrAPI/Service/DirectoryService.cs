using DiiagramrAPI.Service.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;

namespace DiiagramrAPI.Service
{
    public class DirectoryService : IDirectoryService
    {
        public void CreateDirectory(string path)
        {
            var directorySecurity = new DirectorySecurity();
            directorySecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            Directory.CreateDirectory(path, directorySecurity);
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
            catch
            {
                return false;
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
            catch
            {
                return new List<string>();
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
            catch
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
            catch
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