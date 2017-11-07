using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiiagramrAPI.Model
{
    [DataContract]
    public class DependencyModel
    {
        [DataMember]
        public virtual string LibraryName { get; private set; }

        [DataMember]
        public virtual string LibraryVersion { get; private set; }

        public DependencyModel(string libraryName, string libraryVersion)
        {
            LibraryName = libraryName;
            LibraryVersion = libraryVersion;
        }
    }
}
