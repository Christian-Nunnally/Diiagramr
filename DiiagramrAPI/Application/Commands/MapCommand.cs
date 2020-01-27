using DiiagramrAPI.Application.Commands.Transacting;
using System.Collections;

namespace DiiagramrAPI.Application.Commands
{
    public class MapCommand : TransactingCommand
    {
        private readonly IReversableCommand _commandToMap;

        public MapCommand(IReversableCommand commandToMap)
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