using System.Collections.Generic;
using System.IO;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IDirectoryService : IDiiagramrService
    {
        string GetCurrentDirectory();

        void CreateDirectory(string path);

        IEnumerable<string> GetDirectories(string path);

        IEnumerable<string> GetFiles(string path, string searchPattern);
        IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption);

        void Move(string fromPath, string toPath);

        bool Exists(string path);

        void Delete(string path, bool recursive);

        void ExtractToDirectory(string from, string to);

        string GetDirectoryName(string path);

        string ReadAllText(string path);
    }
}
