namespace DiiagramrModel
{
    using System.Runtime.Serialization;

    [DataContract]
    public class NodeLibrary
    {
        public NodeLibrary()
        {
        }

        public NodeLibrary(string name, string downloadPath, int majorVersion, int minorVersion, int patch)
        {
            Name = name;
            DownloadPath = downloadPath;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            Patch = patch;
        }

        [DataMember]
        public string DownloadPath { get; set; }

        [DataMember]
        public int MajorVersion { get; set; }

        [DataMember]
        public int MinorVersion { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PathOnDisk { get; set; }

        [DataMember]
        public int Patch { get; set; }

        public bool IsNewerVersionThan(NodeLibrary otherLibrary)
        {
            if (otherLibrary.MinorVersion < MinorVersion)
            {
                return true;
            }

            return otherLibrary.Patch < Patch;
        }

        public override string ToString()
        {
            return $"{Name} - {MajorVersion}.{MinorVersion}.{Patch}";
        }
    }
}