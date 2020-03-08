using DiiagramrAPI.Editor.Diagrams;
using Stylet;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.Application.Dialogs
{
    public class HelpDialog : Dialog
    {
        private readonly string _defaultVisibleHelpText = "No help available";

        public HelpDialog(Node node)
        {
            Node = node;
            if (TryGetHelpFromNode(node, out var help))
            {
                _defaultVisibleHelpText = help.HelpText;
            }
            Title = node.Name + " Node - Help";
            VisibleHelpText = _defaultVisibleHelpText;
        }

        public override int MaxHeight => 400;

        public override int MaxWidth => 400;

        public override string Title { get; set; }

        public Node Node { get; set; }

        public string VisibleHelpText { get; set; }

        public void PreviewMouseMoveHandler()
        {
            VisibleHelpText = TryGetHelpAttributeFromViewModel(GetViewModelMouseIsOver(), out var helpAttribute)
                ? helpAttribute.HelpText ?? string.Empty
                : _defaultVisibleHelpText;
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
            help = Node?.GetType()
                .GetMethod(inputTerminal.Name)
                ?.GetCustomAttributes(typeof(HelpAttribute), true)
                .FirstOrDefault() as HelpAttribute;
            return help is object;
        }

        private bool TryGetHelpFromNode(Node node, out HelpAttribute help)
        {
            help = node.GetType().GetCustomAttributes(typeof(HelpAttribute), true).FirstOrDefault() as HelpAttribute;
            return help is object;
        }
    }
}