using System.Collections.Generic;
using System.IO;
using System.Linq;
using Diiagramr.Service.Interfaces;

namespace Diiagramr.Service
{
    public class DirectoryService : IDirectoryService
    {
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public IList<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path).ToList();
        }

        public void Move(string fromPath, string toPath)
        {
            Directory.Move(fromPath, toPath);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }
    }
}
