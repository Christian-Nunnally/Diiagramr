﻿using DiiagramrAPI.Service.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands
{
    public class ShellCommandFactory
    {
        private readonly Dictionary<string, IShellCommand> _commands = new Dictionary<string, IShellCommand>();
        private readonly IApplicationShell _shell;

        public ShellCommandFactory(Func<IApplicationShell> shellFactory, Func<IEnumerable<IShellCommand>> commandsFactory)
        {
            ShellCommand.CommandManager = this;
            _shell = shellFactory.Invoke();
            var commands = commandsFactory.Invoke().OrderBy(c => c.Weight);
            SetupCommands(commands);
        }

        public IEnumerable<IShellCommand> Commands => _commands.Values;

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

        public void ExecuteCommand(IShellCommand command)
        {
            ExecuteCommand(command, null);
        }

        public void ExecuteCommand(IShellCommand command, object parameter)
        {
            command.Execute(_shell, parameter);
        }

        private string GenerateCommandPath(IShellCommand command)
        {
            if (command.Parent == null)
            {
                return command.Name;
            }

            return $"{command.Parent}:{command.Name}";
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
                    if (_commands[commandPath].Weight < command.Weight)
                    {
                        _commands[commandPath] = command;
                    }
                }
            }
        }
    }
}