using DiiagramrModel;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectLoadSave : IService
    {
        ProjectModel Open(string fileName);

        void Save(ProjectModel project, string fileName);
    }
}