using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Editor;
using Stylet;
using StyletIoC;
using System.Reflection;

namespace DiiagramrApplication.Application
{
    public class Bootstrapper : Bootstrapper<Shell>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            BootstrapperUtilities.BindServices(builder);
            BootstrapperUtilities.LoadColorInformation();

            builder.Bind<IViewManager>().To<DiiagramrViewManager>().InSingletonScope();
            builder.Assemblies.Add(Assembly.Load(nameof(DiiagramrAPI)));
            builder.Bind<LibraryManagerDialog>().To<LibraryManagerDialog>().InSingletonScope();
            builder.Bind<DialogHost>().To<DialogHost>().InSingletonScope();
            builder.Bind<ScreenHost>().To<ScreenHost>().InSingletonScope();
            builder.Bind<ContextMenu>().To<ContextMenu>().InSingletonScope();
            builder.Bind<ITransactor>().To<GlobalTransactor>().InSingletonScope();
            builder.Bind<NodeServiceProvider>().To<NodeServiceProvider>().InSingletonScope();
            builder.Bind<DiagramWell>().To<DiagramWell>().InSingletonScope();
        }
    }
}