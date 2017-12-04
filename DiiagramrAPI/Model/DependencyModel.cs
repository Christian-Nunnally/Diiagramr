using System.Runtime.Serialization;

namespace DiiagramrAPI.Model
{
    [DataContract]
    public class DependencyModel
    {

        public DependencyModel(string libraryName, int majorLibraryVersion)
        {
            LibraryName = libraryName;
            MajorLibraryVersion = majorLibraryVersion;
        }

        [DataMember]
        public virtual string LibraryName { get; private set; }

        [DataMember]
        public virtual int MajorLibraryVersion { get; private set; }
    }
}