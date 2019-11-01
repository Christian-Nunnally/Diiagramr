using DiiagramrAPI.Shell.Commands.Transacting;
using System.Collections;

namespace DiiagramrAPI.Shell.Commands
{
    public class MapCommand : TransactingCommand
    {
        private readonly ICommand _commandToMap;

        public MapCommand(ICommand commandToMap)
        {
            _commandToMap = commandToMap;
        }

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
