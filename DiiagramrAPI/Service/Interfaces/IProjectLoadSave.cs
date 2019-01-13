using DiiagramrAPI.Model;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectLoadSave : IDiiagramrService
    {
        ProjectModel Open(string fileName);

        void Save(ProjectModel project, string fileName);
    }
}
