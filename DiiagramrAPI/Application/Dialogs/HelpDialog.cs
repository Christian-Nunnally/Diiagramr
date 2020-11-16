using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// A dialog capable of showing help information.
    /// </summary>
    public class HelpDialog : Dialog
    {
        private readonly string _defaultVisibleHelpText = "No help available";

        /// <summary>
        /// Creates a new instance of <see cref="HelpDialog"/>.
        /// </summary>
        /// <param name="node">The node to get help for.</param>
        public HelpDialog(Node node)
        {
            Node = node;
            if (TryGetHelpFromNode(node, out var help))
            {
                _defaultVisibleHelpText = help.HelpText;
            }
            VisibleHelpTitle = node.Name;
            VisibleHelpText = _defaultVisibleHelpText;
        }

        /// <inheritdoc/>
        public override int MaxHeight => 400;

        /// <inheritdoc/>
        public override int MaxWidth => 340;

        /// <inheritdoc/>
        public override string Title { get; set; } = "Help";

        /// <summary>
        /// The node to display in the help visiual section.
        /// </summary>
        public Node Node { get; set; }

        /// <summary>
        /// The help text of the help description section.
        /// </summary>
        public string VisibleHelpText { get; set; }

        /// <summary>
        /// The title text of the help description section.
        /// </summary>
        public string VisibleHelpTitle { get; set; }

        public void PreviewMouseMoveHandler()
        {
            var terminal = GetViewModelMouseIsOver() as Terminal;
            VisibleHelpTitle = terminal?.Name ?? Node.Name;
            VisibleHelpText = TryGetHelpAttributeFromViewModel(terminal, out var helpAttribute)
                ? helpAttribute.HelpText ?? string.Empty
                : _defaultVisibleHelpText;

            // TODO: Add command bar action that links to the wiki.
        }

        private static Screen GetViewModelMouseIsOver()
        {
            var elementMouseIsOver = Mouse.DirectlyOver as FrameworkElement;
            if (elementMouseIsOver?.DataContext is Screen viewModelMouseIsOver)
            {
                return viewModelMouseIsOver;
            }
            var contentPresenter = elementMouseIsOver?.DataContext as ContentPresenter;
            return contentPresenter?.DataContext as Screen;
        }

        private bool TryGetHelpAttributeFromViewModel(Screen viewModel, out HelpAttribute helpAttribute)
        {
            switch (viewModel)
            {
                case OutputTerminal outputTerminal:
                    return TryGetHelpFromOutputTerminal(outputTerminal, out helpAttribute);

                case InputTerminal inputTerminal:
                    return TryGetHelpFromInputTerminal(inputTerminal, out helpAttribute);
            }
            helpAttribute = null;
            return false;
        }

        private bool TryGetHelpFromOutputTerminal(OutputTerminal outputTerminal, out HelpAttribute help)
        {
            help = Node?.GetType()
                .GetProperty(outputTerminal.Name)
                ?.GetCustomAttributes(typeof(HelpAttribute), true)
                .FirstOrDefault() as HelpAttribute;
            return help is object;
        }

        private bool TryGetHelpFromInputTerminal(InputTerminal inputTerminal, out HelpAttribute help)
        {
            var nodeType = Node?.GetType();
            MemberInfo terminalMemberInfo = nodeType?.GetMethod(inputTerminal.Name);
            if (terminalMemberInfo == null)
            {
                terminalMemberInfo = nodeType?.GetProperty(inputTerminal.Name);
            }
            help = terminalMemberInfo?.GetCustomAttributes(typeof(HelpAttribute), true)?.FirstOrDefault() as HelpAttribute;
            return help is object;
        }

        private bool TryGetHelpFromNode(Node node, out HelpAttribute help)
        {
            help = node.GetType().GetCustomAttributes(typeof(HelpAttribute), true).FirstOrDefault() as HelpAttribute;
            return help is object;
        }
    }
}