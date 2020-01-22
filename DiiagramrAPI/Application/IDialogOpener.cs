using DiiagramrAPI.Service;

namespace DiiagramrAPI.Application
{
    public interface IDialogOpener : ISingletonService
    {
        void OpenDialog(ShellDialog dialog);
    }
}