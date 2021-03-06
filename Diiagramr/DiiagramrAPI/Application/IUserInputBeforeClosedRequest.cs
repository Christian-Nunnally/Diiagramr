﻿using System;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Interface that can be added to shell screen view models to allow them to prompt for user input before the shell closes them.
    /// </summary>
    public interface IUserInputBeforeClosedRequest
    {
        /// <summary>
        /// Calls the <paramref name="continuation"/> if this object is willing to close.
        /// </summary>
        /// <param name="continuation">An action to call is the application is allowed to close.</param>
        void ContinueIfCanClose(Action continuation);
    }
}