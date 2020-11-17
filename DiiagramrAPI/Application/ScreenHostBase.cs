using Stylet;
using System;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// A class that hosts other instances of <see cref="IScreen"/> in a stack like fashion.
    /// </summary>
    public abstract class ScreenHostBase : Conductor<IScreen>.StackNavigation
    {
        /// <summary>
        /// Shows the given screen.
        /// </summary>
        /// <param name="screen">The screen to show.</param>
        public abstract void ShowScreen(IScreen screen);

        /// <summary>
        /// Closes all screens, allowing each screen a chance to interupt the close and prompt the user if it implements <see cref="IUserInputBeforeClosedRequest"/>.
        /// </summary>
        /// <param name="continuation">The continuation to run if all of the screens close.</param>
        public abstract void InteractivelyCloseAllScreens(Action continuation);
    }
}