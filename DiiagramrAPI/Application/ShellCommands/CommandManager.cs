using DiiagramrAPI.Service.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands
{
    public class CommandManager
    {
        private readonly Dictionary<string, IShellCommand> _commands = new Dictionary<string, IShellCommand>();

        public CommandManager(Func<IEnumerable<IShellCommand>> commandsFactory)
        {
            CommandExecutor.CommandManager = this;
            var commands = commandsFactory();
            SetupCommands(commands);
        }

        public IEnumerable<IShellCommand> Commands => _commands.Values;

        public IEnumerable<IToolbarCommand> ToolbarCommands => _commands.Values.OfType<IToolbarCommand>();

        public void ExecuteCommand(string commandID, object parameter)
        {
            if (_commands.ContainsKey(commandID))
            {
                ExecuteCommand(_commands[commandID], parameter);
            }
        }

        public void ExecuteCommand(string commandID)
        {
            ExecuteCommand(commandID, null);
        }

        public void ExecuteCommand(IShellCommand command)
        {
            ExecuteCommand(command, null);
        }

        public void ExecuteCommand(IShellCommand command, object parameter)
        {
            command.Execute(parameter);
        }

        private string GenerateCommandPath(IShellCommand command)
        {
            if (command is IToolbarCommand toolbarCommand)
            {
                if (toolbarCommand.ParentName == null)
                {
                    return command.Name;
                }
                return $"{toolbarCommand.ParentName}:{command.Name}";
            }
            return command.Name;
        }

        private void SetupCommands(IEnumerable<IShellCommand> commands)
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
                    if (_commands[commandPath] is IToolbarCommand toolbarCommand && command is IToolbarCommand toolbarCommand2)
                    {
                        if (toolbarCommand.Weight < toolbarCommand2.Weight)
                        {
                            _commands[commandPath] = command;
                        }
                        else
                        {
                            _commands[commandPath] = null;
                        }
                    }
                    else
                    {
                        _commands[commandPath] = command;
                    }
                }
            }
        }
    }
}