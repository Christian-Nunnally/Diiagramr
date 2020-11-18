namespace DiiagramrModel
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a library of nodes, essentially a one level deep namespace for nodes.
    /// </summary>
    [DataContract]
    public class NodeLibrary
    {
        /// <summary>
        /// Creates a new blank instance of <see cref="NodeLibrary"/>.
        /// </summary>
        public NodeLibrary()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NodeLibrary"/>.
        /// </summary>
        /// <param name="name">The libraries name.</param>
        /// <param name="downloadPath">The online path to download the library.</param>
        /// <param name="majorVersion">The major version number of the library.</param>
        /// <param name="minorVersion">The minor version number of the library.</param>
        /// <param name="patch">The patch number of the library.</param>
        public NodeLibrary(string name, string downloadPath, int majorVersion, int minorVersion, int patch)
        {
            Name = name;
            DownloadPath = downloadPath;
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
            Patch = patch;
        }

        /// <summary>
        /// Gets or sets the name number of the library.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the online path to download the library.
        /// </summary>
        [DataMember]
        public string DownloadPath { get; set; }

        /// <summary>
        /// Gets or sets the major version number of the library.
        /// </summary>
        [DataMember]
        public int MajorVersion { get; set; }

        /// <summary>
        /// Gets or sets the minor version number of the library.
        /// </summary>
        [DataMember]
        public int MinorVersion { get; set; }

        /// <summary>
        /// Gets or sets the patch number of the library.
        /// </summary>
        [DataMember]
        public int Patch { get; set; }

        /// <summary>
        /// Gets or sets the local path to this library.
        /// </summary>
        [DataMember]
        public string PathOnDisk { get; set; }

        /// <summary>
        /// Gets whether this library is a newer version then another library.
        /// </summary>
        /// <param name="otherLibrary">The other library to check the version agaisnt.</param>
        /// <returns>True if this library is a newer version than the other library.</returns>
        public bool IsNewerVersionThan(NodeLibrary otherLibrary) =>
            otherLibrary.MajorVersion == MajorVersion
                ? otherLibrary.MinorVersion == MinorVersion
                    ? otherLibrary.Patch < Patch
                    : MinorVersion < MinorVersion
                : otherLibrary.MajorVersion < MajorVersion;

        /// <inheritdoc/>
        public override string ToString() => $"{Name} - {MajorVersion}.{MinorVersion}.{Patch}";
    }
}