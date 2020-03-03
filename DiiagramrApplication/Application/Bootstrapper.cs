using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Application.Dialogs;
using DiiagramrAPI.Application.ShellCommands.StartupCommands;
using DiiagramrAPI.Application.Tools;
using DiiagramrAPI.Project;
using DiiagramrAPI.Service.Application;
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
            builder.Bind<DialogHostBase>().To<DialogHost>().InSingletonScope();
            builder.Bind<ScreenHostBase>().To<ScreenHost>().InSingletonScope();
            builder.Bind<ContextMenuBase>().To<ContextMenu>().InSingletonScope();
            builder.Bind<ToolbarBase>().To<Toolbar>().InSingletonScope();
            builder.Bind<ITransactor>().To<GlobalTransactor>().InSingletonScope();
            builder.Bind<NodeServiceProvider>().To<NodeServiceProvider>().InSingletonScope();
            builder.Bind<DiagramWell>().To<DiagramWell>().InSingletonScope();
            builder.Bind<IShellCommand>().To<VisualDropStartScreenCommand>().WithKey("startCommand");
        }
    }
}