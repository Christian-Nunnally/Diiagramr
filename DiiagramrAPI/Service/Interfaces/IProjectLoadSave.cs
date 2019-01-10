using DiiagramrAPI.Model;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectLoadSave : IDiiagramrService
    {
        void Save(ProjectModel project, string fileName);

        ProjectModel Open(string fileName);
    }
}
