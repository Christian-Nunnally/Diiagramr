using System;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Interface that can be added to shell screen view models to allow them to prompt for user input before the shell closes them.
    /// </summary>
    internal interface IUserInputBeforeClosedRequest
    {
        /// <summary>
        /// Calls the <paramref name="continuation"/> if this object is willing to close.
        /// </summary>
        /// <param name="continuation"></param>
        void ContinueIfCanClose(Action continuation);
    }
}