using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Diiagramr.Model;

namespace Diiagramr.Service.Interfaces
{
    public interface IProjectLoadSave
    {
        void Save(ProjectModel project, string name);

        ProjectModel Open(string fileName);
    }
}
