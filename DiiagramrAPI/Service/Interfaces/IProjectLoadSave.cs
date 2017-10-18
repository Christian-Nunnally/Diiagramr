using DiiagramrAPI.Model;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IProjectLoadSave
    {
        void Save(ProjectModel project, string name);

        ProjectModel Open(string fileName);
    }
}
