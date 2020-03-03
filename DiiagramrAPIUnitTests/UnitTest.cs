using DiiagramrAPI.Application;
using DiiagramrAPI.Application.ShellCommands;
using DiiagramrAPI.Application.Tools;
using StyletIoC;
using System;

namespace DiiagramrAPIUnitTests
{
    public class UnitTest
    {
        protected T CreateUnitTestInstance<T>(string key = null)
        {
            var builder = new StyletIoCBuilder();
            builder.Bind<Shell>().ToSelf();
            builder.Bind<ToolbarBase>().To<FakeToolbar>();
            builder.Bind<ShellCommandBase>().To<FakeCommand>().WithKey("startCommand");
            builder.Bind<IHotkeyCommander>().To<FakeHotkeyCommander>();
            builder.Bind<ContextMenuBase>().To<FakeContextMenu>();
            builder.Bind<ScreenHostBase>().To<FakeScreenHost>();
            builder.Bind<DialogHostBase>().To<FakeDialogHost>();

            var container = builder.BuildContainer();
            return container.GetTypeOrAll<T>(key);
        }

        protected Func<T> CreateFactoryFor<T>(T shellCommand, string key = null)
        {
            return shellCommand is object ? new Func<T>(() => shellCommand) : () => CreateUnitTestInstance<T>(key);
        }
    }
}