using DiiagramrAPI.Service;
using DiiagramrModel;

namespace DiiagramrAPI.Project
{
    public interface IProjectLoadSave : IService
    {
        ProjectModel Open(string fileName);

        void Save(ProjectModel project, string fileName);
    }
}