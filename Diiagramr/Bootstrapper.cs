using System.Reflection;
using DiiagramrAPI.CustomControls;
using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel;
using DiiagramrAPI.ViewModel.ShellScreen;
using Stylet;
using StyletIoC;

namespace Diiagramr
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<IViewManager>().To<CustomViewManager>().InSingletonScope();
            builder.Bind<ViewManager>().To<CustomViewManager>().InSingletonScope();
            builder.Assemblies.Add(Assembly.Load(nameof(DiiagramrAPI)));
            builder.Bind<IDirectoryService>().To<DirectoryService>();
            builder.Bind<IProjectLoadSave>().To<ProjectLoadSave>();
            builder.Bind<IProjectFileService>().To<ProjectFileService>().InSingletonScope();
            builder.Bind<IProjectManager>().To<ProjectManager>().InSingletonScope();
            builder.Bind<IProvideNodes>().To<NodeProvider>().InSingletonScope();
            builder.Bind<IPluginWatcher>().To<PluginWatcher>().InSingletonScope();
            builder.Bind<LibraryManagerScreenViewModel>().To<LibraryManagerScreenViewModel>().InSingletonScope();
            builder.Bind<IFileDialog>().To<OpenFileDialog>().WithKey("open");
            builder.Bind<IFileDialog>().To<SaveFileDialog>().WithKey("save");
        }
    }
}