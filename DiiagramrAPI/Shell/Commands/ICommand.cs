using System;

namespace DiiagramrAPI.Shell.Commands
{
    /// <summary>
    /// A command to wrap around complex editor operations.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Do the action.
        /// </summary>
        /// <param name="transactor">Transactor to provide extensibility to the command infastructure.</param>
        /// <param name="parameter">General purpose command parameter.</param>
        /// <returns>An action that will undo what execute did.</returns>
        Action Execute(object parameter);
    }
}