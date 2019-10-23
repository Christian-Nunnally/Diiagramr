using DiiagramrAPI.Service.Commands;
using DiiagramrAPI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Service
{
    // todo: maybe rename to commandfactory?
    public class CommandManager
    {
        private readonly IShell _shell;
        private Dictionary<string, IDiiagramrCommand> _commands = new Dictionary<string, IDiiagramrCommand>();
        public IEnumerable<IDiiagramrCommand> Commands => _commands.Values;

        public CommandManager(Func<IShell> shellFactory, Func<IEnumerable<IDiiagramrCommand>> commandsFactory)
        {
            ShellCommand.CommandManager = this;
            _shell  = shellFactory.Invoke();
            var commands = commandsFactory.Invoke().OrderBy(c => c.Weight);
            SetupCommands(commands);
        }

        private void SetupCommands(IEnumerable<IDiiagramrCommand> commands)
        {
            foreach (var command in commands)
            {
                var commandPath = GenerateCommandPath(command);
                if (!_commands.ContainsKey(commandPath))
                {
                    _commands.Add(commandPath, command);
                }
                else
                {
                    if (_commands[commandPath].Weight < command.Weight)
                    {
                        _commands[commandPath] = command;
                    }
                }
            }
        }

        private string GenerateCommandPath(IDiiagramrCommand command)
        {
            if (command.Parent == null)
            {
                return command.Name;
            }

            return $"{command.Parent}:{command.Name}";
        }

        public void ExecuteCommand(string commandID, object parameter)
        {
            if (_commands.ContainsKey(commandID))
            {
                ExecuteCommand(_commands[commandID], parameter);
            }
            else
            {
                // TODO: Log that a command was executed that doesn't exist.
            }
        }

        public void ExecuteCommand(string commandID)
        {
            ExecuteCommand(commandID, null);
        }

        public void ExecuteCommand(IDiiagramrCommand command)
        {
            ExecuteCommand(command, null);
        }

        public void ExecuteCommand(IDiiagramrCommand command, object parameter)
        {
            if (command.CanExecute(_shell))
            {
                command.Execute(_shell, parameter);
            }
        }
    }
}
