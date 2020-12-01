namespace DiiagramrAPI.Service.Application
{
    /// <summary>
    /// Interface for a basic command.
    /// </summary>
    public interface IShellCommand : ISingletonService
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Used to cache the value last returned by <see cref="CanExecute"/>.
        /// </summary>
        bool CachedCanExecute { get; set; }

        /// <summary>
        /// Whether or not this command can execute at the moment.
        /// </summary>
        /// <returns>True is this command can be executed.</returns>
        bool CanExecute();

        /// <summary>
        /// Execute the comand.
        /// </summary>
        /// <param name="parameter">The parameter to pass in to the command.</param>
        void Execute(object parameter);
    }
}