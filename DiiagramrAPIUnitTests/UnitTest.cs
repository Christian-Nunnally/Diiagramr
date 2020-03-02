using DiiagramrAPI.Application;
using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using StyletIoC;

namespace DiiagramrAPIUnitTests
{
    public class UnitTest
    {
        protected T CreateUnitTestInstance<T>()
        {
            var builder = new StyletIoCBuilder();
            builder.Bind<Shell>().ToSelf();
            builder.Bind<ToolbarBase>().To<FakeToolbar>();
            builder.Bind<ShellCommandBase>().To<FakeStartCommand>().WithKey("startCommand");
            builder.Bind<IHotkeyCommander>().To<FakeHotkeyCommander>();

            var container = builder.BuildContainer();
            return container.GetTypeOrAll<T>();
        }
    }
}