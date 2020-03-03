using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Service.Editor;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DiiagramrAPI.Application.ShellCommands.DiagnosticsCommands
{
    public class CreateHelpFileTemplateCommand : ShellCommandBase, IToolbarCommand
    {
        private readonly INodeProvider _nodeProvider;

        public CreateHelpFileTemplateCommand(Func<INodeProvider> nodeProviderFactory)
        {
            _nodeProvider = nodeProviderFactory();
        }

        public override string Name => "Create Help File Template";

        public string ParentName => "Diagnostics";

        public float Weight => 1f;

        public void OpenDirectory()
        {
            Process.Start("explorer.exe", ".");
        }

        protected override bool CanExecuteInternal()
        {
            return true;
        }

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
    }
}