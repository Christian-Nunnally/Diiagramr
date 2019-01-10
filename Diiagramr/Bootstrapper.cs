using DiiagramrAPI;
using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel;
using Stylet;
using StyletIoC;
using System.Reflection;

namespace Diiagramr
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            BootstrapperUtilities.BindServices(builder);

            builder.Bind<IViewManager>().To<DiiagramrViewManager>().InSingletonScope();
            builder.Assemblies.Add(Assembly.Load(nameof(DiiagramrAPI)));
            builder.Bind<ColorTheme>().To<ColorTheme>().InSingletonScope();
            builder.Bind<LibraryManagerWindowViewModel>().To<LibraryManagerWindowViewModel>().InSingletonScope();
        }
    }
}