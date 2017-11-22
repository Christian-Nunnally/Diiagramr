using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stylet;

namespace Diiagramr.View.ShellWindow
{
    public abstract class AbstractShellWindow : Screen
    {
        public abstract int MaxWidth { get; }
        public abstract int MaxHeight { get; }
        public abstract string Title { get; }

        public event Action<AbstractShellWindow> OpenWindow;

        protected void OpenOtherWindow(AbstractShellWindow otherWindow)
        {
            OpenWindow?.Invoke(otherWindow);
        }
    }
}
