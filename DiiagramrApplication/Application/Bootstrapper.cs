using DiiagramrAPI.Application;
using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Application.Tools;
using Stylet;
using StyletIoC;
using System.Reflection;

namespace Diiagramr.Application
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            BootstrapperUtilities.BindServices(builder);
            BootstrapperUtilities.LoadColorInformation();

            builder.Bind<IViewManager>().To<DiiagramrViewManager>().InSingletonScope();
            builder.Assemblies.Add(Assembly.Load(nameof(DiiagramrAPI)));
            builder.Bind<LibraryManagerWindow>().To<LibraryManagerWindow>().InSingletonScope();
            builder.Bind<IApplicationShell>().To<ApplicationShell>().InSingletonScope();
            builder.Bind<ITransactor>().To<GlobalTransactor>().InSingletonScope();
        }
    }
}