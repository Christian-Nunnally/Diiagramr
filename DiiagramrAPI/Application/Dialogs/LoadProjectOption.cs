using PropertyChanged;

namespace DiiagramrAPI2.Application.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    public class LoadProjectOption
    {
        public LoadProjectOption(string path)
        {
            Path = path;
            var splitPath = path.Split("\\");
            Name = splitPath[^1];
        }

        public string Path { get; set; }

        public string Name { get; set; }

        public static LoadProjectOption Create(string path) => new LoadProjectOption(path);
    }
}