using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Dialogs
{
    public class MessageBox : Dialog
    {
        private MessageBox(string title, string message, IList<DialogCommandBarCommand> choices)
        {
            Title = title;
            Message = message;
            foreach (var choice in choices)
            {
                CommandBarCommands.Add(new DialogCommandBarCommand(choice.Label, () => { CloseDialog(); choice.Action(); }));
            }
        }

        public override int MaxHeight => 45;

        public override int MaxWidth => 220;

        public override string Title { get; set; }

        public string Message { get; set; }

        public class Builder
        {
            private readonly List<DialogCommandBarCommand> _choices = new List<DialogCommandBarCommand>();
            private readonly string _name;
            private readonly string _message;

            public Builder(string title, string message)
            {
                _name = title;
                _message = message;
            }

            public Builder WithChoice(string choiceLabel, Action action)
            {
                _choices.Add(new DialogCommandBarCommand(choiceLabel, action));
                return this;
            }

            public MessageBox Build() => new MessageBox(_name, _message, _choices);
        }
    }
}