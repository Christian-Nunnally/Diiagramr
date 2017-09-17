using System.Collections.Generic;

namespace Diiagramr.Service.Interfaces
{
    public interface IDirectoryService
    {
        string GetCurrentDirectory();

        void CreateDirectory(string path);

        IList<string> GetDirectories(string path);

        void Move(string fromPath, string toPath);

        bool Exists(string path);

        void Delete(string path, bool recursive);
    }
}
