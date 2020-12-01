using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// A dialog to present the user with a list of true/false options.
    /// </summary>
    public class OptionDialog : Dialog
    {
        private readonly Option _organizeNodesOption = new Option(
            "Organize nodes by library.",
            () => NodePalette.ShouldSortNodesByLibraryInsteadOfByCategory = true,
            () => NodePalette.ShouldSortNodesByLibraryInsteadOfByCategory = false);

        private readonly Option _visualizeDataPropagationOption = new Option(
            "Organize nodes by library.",
            () => Wire.ShowDataPropagation = true,
            () => Wire.ShowDataPropagation = false);

        /// <summary>
        /// Creates a new instance of <see cref="OptionDialog"/>
        /// </summary>
        public OptionDialog()
        {
            var nodePickerOptionCategory = new OptionCategory("Node Picker");
            nodePickerOptionCategory.AddOption(_organizeNodesOption);
            OptionCategories.Add(nodePickerOptionCategory);

            var debuggingOptionCategory = new OptionCategory("Debugging");
            debuggingOptionCategory.AddOption(_visualizeDataPropagationOption);
            OptionCategories.Add(debuggingOptionCategory);
        }

        /// <summary>
        /// The list of option categories visible in the dialog.
        /// </summary>
        public ObservableCollection<OptionCategory> OptionCategories { get; set; } = new ObservableCollection<OptionCategory>();

        /// <inheritdoc/>
        public override int MaxHeight => 350;

        /// <inheritdoc/>
        public override int MaxWidth => 280;

        /// <inheritdoc/>
        public override string Title { get; set; } = "Options";

        /// <summary>
        /// Occurs when an option check box is checked.
        /// </summary>
        /// <param name="sender">The checkbox that was checked.</param>
        /// <param name="e">The event arguments</param>
        public void OptionCheckboxCheckedHandler(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement.DataContext is Option option)
            {
                option.Enable();
            }
        }

        /// <summary>
        /// Occurs when an option check box is unchecked.
        /// </summary>
        /// <param name="sender">The checkbox that was unchecked.</param>
        /// <param name="e">The event arguments</param>
        public void OptionCheckboxUncheckedHandler(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement.DataContext is Option option)
            {
                option.Disable();
            }
        }

        /// <summary>
        /// Helper view model class for option categories.
        /// </summary>
        public class OptionCategory
        {
            /// <summary>
            /// Creates a new instance of <see cref="OptionCategory"/>.
            /// </summary>
            /// <param name="name">The name of this cateogory of options.</param>
            public OptionCategory(string name)
            {
                Name = name;
            }

            /// <summary>
            /// The list of options that should be visible under this cateogry.
            /// </summary>
            public ObservableCollection<Option> Options { get; set; } = new ObservableCollection<Option>();

            /// <summary>
            /// The name of this category of options.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Adds a new option to the category.
            /// </summary>
            /// <param name="option">The option to add.</param>
            public void AddOption(Option option)
            {
                Options.Add(option);
            }
        }

        /// <summary>
        /// Helper view model class for individual optionsi in the options dialog.
        /// </summary>
        public class Option
        {
            /// <summary>
            /// Creates a new instance of <see cref="Option"/>.
            /// </summary>
            /// <param name="name">The user visible name of the option.</param>
            /// <param name="enable">The action to take to enable this option.</param>
            /// <param name="disable">The action to take to disable this option.</param>
            public Option(string name, Action enable, Action disable)
            {
                Name = name;
                Enable = enable;
                Disable = disable;
            }

            /// <summary>
            /// Gets or sets the user visible name of the option.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets or sets the action to take to enable this option.
            /// </summary>
            public Action Enable { get; set; }

            /// <summary>
            /// Gets or sets the action to take to disable this option.
            /// </summary>
            public Action Disable { get; set; }
        }
    }
}