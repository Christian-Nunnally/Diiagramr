using System;
using System.Collections.Generic;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// A message box that can be shown to the user.
    /// </summary>
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

        /// <inheritdoc/>
        public override int MaxHeight => 45;

        /// <inheritdoc/>
        public override int MaxWidth => 220;

        /// <inheritdoc/>
        public override string Title { get; set; }

        /// <summary>
        /// The message to display in the message box body.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A <see cref="MessageBox"/> builder for creating instances using the builder pattern.
        /// </summary>
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