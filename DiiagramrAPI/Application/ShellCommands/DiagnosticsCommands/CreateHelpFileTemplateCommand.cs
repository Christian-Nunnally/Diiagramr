using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    /// <summary>
    /// Development only command to help autogenerate wiki pages.
    /// </summary>
    public class CreateHelpFileTemplateCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly INodeProvider _nodeProvider;

        public CreateHelpFileTemplateCommand(Func<INodeProvider> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
        }

        /// <inheritdoc/>
        public override string Name => "Create Help File Template";

        /// <inheritdoc/>
        public string ParentName => "Diagnostics";

        /// <inheritdoc/>
        public float Weight => 1f;

        /// <inheritdoc/>
        protected override bool CanExecuteInternal()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void ExecuteInternal(object parameter)
        {
            var fileName = "HelpTemplate.md";
            using (var sr = new StreamWriter(fileName))
            {
                var nodes = _nodeProvider.GetRegisteredNodes();
                foreach (var node in nodes)
                {
                    sr.WriteLine($"# {node.Name}");
                    if (node.GetType().GetCustomAttributes(typeof(HelpAttribute), true).FirstOrDefault() is HelpAttribute help)
                    {
                        sr.WriteLine($"{help.HelpText}");
                    }
                    else
                    {
                        sr.WriteLine($"{node.Name} description.");
                    }
                    sr.WriteLine($"## Terminals");
                    foreach (var inputTerminal in node.Terminals.OfType<InputTerminal>())
                    {
                        sr.WriteLine($"### {inputTerminal.Name}");
                        sr.WriteLine($"{inputTerminal.Name} description.");
                    }
                    foreach (var outputTerminal in node.Terminals.OfType<OutputTerminal>())
                    {
                        sr.WriteLine($"### {outputTerminal.Name}");
                        sr.WriteLine($"{outputTerminal.Name} description.");
                    }
                }
                sr.Close();
            }
            OpenDirectory();
        }

        private void OpenDirectory()
        {
            Process.Start("explorer.exe", ".");
        }
    }
}