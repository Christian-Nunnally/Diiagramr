using DiiagramrAPI.Editor.Diagrams;
using DiiagramrAPI.Editor.Interactors;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace DiiagramrAPI.Application.Dialogs
{
    public interface IOptionProvider
    {
    }

    public class OptionDialog : Dialog
    {
        public OptionDialog()
        {
            var organizeNodesByLibraryOption = new Option(
                "Organize nodes by library.",
                () => NodePalette.ShouldSortNodesByLibraryInsteadOfByCategory = true,
                () => NodePalette.ShouldSortNodesByLibraryInsteadOfByCategory = false);
            var nodePickerOptionCategory = new OptionCategory("Node Picker");
            nodePickerOptionCategory.AddOption(organizeNodesByLibraryOption);
            OptionCategories.Add(nodePickerOptionCategory);

            var visualizeDataPropagationOption = new Option(
                "Visualize data propagation.",
                () => Wire.ShowDataPropagation = true,
                () => Wire.ShowDataPropagation = false);
            var debuggingOptionCategory = new OptionCategory("Debugging");
            debuggingOptionCategory.AddOption(visualizeDataPropagationOption);
            OptionCategories.Add(debuggingOptionCategory);
        }

        public ObservableCollection<OptionCategory> OptionCategories { get; set; } = new ObservableCollection<OptionCategory>();
        public bool IsRestartRequired { get; set; }

        public override int MaxHeight => 350;

        public override int MaxWidth => 280;

        public override string Title { get; set; } = "Options";

        public void OptionCheckboxCheckedHandler(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement.DataContext is Option option)
            {
                option.EnableOption();
            }
        }

        public void OptionCheckboxUncheckedHandler(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement.DataContext is Option option)
            {
                option.DisableOption();
            }
        }

        public class OptionCategory
        {
            public OptionCategory(string name)
            {
                Name = name;
            }

            public ObservableCollection<Option> Options { get; set; } = new ObservableCollection<Option>();
            public string Name { get; }

            public void AddOption(Option option)
            {
                Options.Add(option);
            }
        }

        public class Option
        {
            public Option(string name, Action enableOption, Action disableOption)
            {
                Name = name;
                EnableOption = enableOption;
                DisableOption = disableOption;
            }

            public string Name { get; }
            public Action EnableOption { get; set; }
            public Action DisableOption { get; set; }

            public void CheckBoxChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
            {
                if (e.NewValue is bool newValue && newValue)
                {
                    EnableOption();
                }
                else
                {
                    DisableOption();
                }
            }
        }
    }
}