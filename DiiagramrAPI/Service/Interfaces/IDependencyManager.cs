using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Service.Interfaces
{
    public interface IDependencyManager
    {
        void SaveDependencies();

        void InstallDependencies();
    }
}
