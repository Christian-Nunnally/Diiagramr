namespace DiiagramrAPI.Service.Application
{
    public interface IShellCommand : ISingletonService
    {
        string Name { get; }

        bool CachedCanExecute { get; set; }

        bool CanExecute();

        void Execute(object parameter);
    }
}