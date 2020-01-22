using DiiagramrAPI.Service;
using DiiagramrModel;

namespace DiiagramrAPI.Project
{
    public interface IProjectLoadSave : ISingletonService
    {
        ProjectModel Open(string fileName);

        void Save(ProjectModel project, string fileName);
    }
}