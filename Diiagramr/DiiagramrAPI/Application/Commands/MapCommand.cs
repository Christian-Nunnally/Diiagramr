using DiiagramrAPI.Application.Commands.Transacting;
using System.Collections;

namespace DiiagramrAPI.Application.Commands
{
    /// <summary>
    /// A <see cref="TransactingCommand"/> that takes a list of parameters as an input and applies another command to each item in the list.
    /// </summary>
    public class MapCommand : TransactingCommand
    {
        private readonly IReversableCommand _commandToMap;

        /// <summary>
        /// Creates a new instance of <see cref="MapCommand"/>.
        /// </summary>
        /// <param name="commandToMap">The command to map a list parameter onto.</param>
        public MapCommand(IReversableCommand commandToMap)
        {
            _commandToMap = commandToMap;
        }

        /// <inheritdoc/>
        protected override void Execute(ITransactor transactor, object parameter)
        {
            if (parameter is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    transactor.Transact(_commandToMap, item);
                }
            }
        }
    }
}