using PropertyChanged;

namespace DiiagramrAPI2.Application.Dialogs
{
    /// <summary>
    /// A helper view model class for displaying project load options.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class LoadProjectOption
    {
        /// <summary>
        /// Creates a new instance of <see cref="LoadProjectOption"/>.
        /// </summary>
        /// <param name="path">The path to the project this option will load.</param>
        public LoadProjectOption(string path)
        {
            Path = path;
            var splitPath = path.Split("\\");
            Name = splitPath[^1];
        }

        /// <summary>
        /// Gets or sets the path to the project this option will load.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets of sets the name of the project that this option will load.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Factory to create a new <see cref="LoadProjectOption"/> instance.
        /// </summary>
        /// <param name="path">The path to the project this option will load.</param>
        /// <returns>The new instance.</returns>
        public static LoadProjectOption Create(string path) => new LoadProjectOption(path);
    }
}