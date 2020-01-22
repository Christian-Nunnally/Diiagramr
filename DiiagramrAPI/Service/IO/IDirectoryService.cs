using System.Collections.Generic;
using System.IO;

namespace DiiagramrAPI.Service.IO
{
    public interface IDirectoryService : ISingletonService
    {
        void CreateDirectory(string path);

        void Delete(string path, bool recursive);

        bool Exists(string path);

        void ExtractToDirectory(string sourcePath, string destinationPath);

        string GetCurrentDirectory();

        IEnumerable<string> GetDirectories(string path);

        string GetDirectoryName(string path);

        IEnumerable<string> GetFiles(string path, string searchPattern);

        IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption);

        void Move(string fromPath, string toPath);

        string ReadAllText(string path);
    }
}