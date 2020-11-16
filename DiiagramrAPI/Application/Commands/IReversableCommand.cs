using System;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A command to wrap around complex editor operations.
    /// </summary>
    public interface IReversableCommand
    {
        /// <summary>
        /// Do the action.
        /// </summary>
        /// <param name="parameter">General purpose command parameter.</param>
        /// <returns>An action that will undo what execute did.</returns>
        Action Execute(object parameter);
    }
}