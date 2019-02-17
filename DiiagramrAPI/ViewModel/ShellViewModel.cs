using DiiagramrAPI.Service.Commands;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.ProjectScreen;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public const string StartCommandId = "start";
        public Stack<AbstractShellWindow> WindowStack = new Stack<AbstractShellWindow>();
        private Dictionary<string, IDiiagramrCommand> _shellCommands = new Dictionary<string, IDiiagramrCommand>();
        private const double ShellRelativePositonYOffSet = -22;
        private const double ShellRelativePositonXOffSet = -5;
        private const double MaximizedWindowChromeRelativePositionAdjustment = -4;

        public ShellViewModel(
            Func<ProjectScreenViewModel> projectScreenViewModelFactory,
            Func<LibraryManagerWindowViewModel> libraryManagerWindowViewModelFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<IEnumerable<IDiiagramrCommand>> commandsFactory,
            Func<ContextMenuViewModel> contextMenuViewModelFactory)
        {
            var commands = commandsFactory.Invoke().OrderBy(c => c.Weight);
            ContextMenuViewModel = contextMenuViewModelFactory.Invoke();
            SetupCommands(commandsFactory.Invoke());
            ProjectScreenViewModel = projectScreenViewModelFactory.Invoke();
            LibraryManagerWindowViewModel = libraryManagerWindowViewModelFactory.Invoke();

            ProjectManager = projectManagerFactory.Invoke();
            ProjectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            ShowScreen(ProjectScreenViewModel);

            ExecuteCommand(StartCommandId);
        }

        public AbstractShellWindow ActiveWindow { get; set; }
        public bool CanSaveAsProject { get; set; }
        public bool CanSaveProject { get; set; }
        public ContextMenuViewModel ContextMenuViewModel { get; set; }
        public bool IsWindowOpen => ActiveWindow != null;
        public LibraryManagerWindowViewModel LibraryManagerWindowViewModel { get; set; }
        public IProjectManager ProjectManager { get; }
        public ProjectScreenViewModel ProjectScreenViewModel { get; set; }
        public ObservableCollection<TopLevelToolBarCommand> TopLevelMenuItems { get; set; }
        public string WindowTitle { get; set; } = "Diiagramr";
        public double Width { get; set; } = 1010;
        public double Height { get; set; } = 830;

        public void CloseWindow()
        {
            if (ActiveWindow != null)
            {
                ActiveWindow.OpenWindow -= OpenWindow;
                ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
            }
        }

        public void ExecuteCommand(IDiiagramrCommand command)
        {
            ExecuteCommand(command, null);
        }

        public void ExecuteCommand(IDiiagramrCommand command, object parameter)
        {
            if (command.CanExecute(this))
            {
                command.Execute(this, parameter);
            }
        }

        public void ExecuteCommand(string commandID)
        {
            if (_shellCommands.ContainsKey(commandID))
            {
                ExecuteCommand(_shellCommands[commandID]);
            }
        }

        public void ExecuteCommandHandler(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            if (control?.DataContext is DiiagramrCommand command)
            {
                var shellRelativePosition = control.TransformToAncestor(Application.Current.MainWindow);
                var correctedRelativePosition = shellRelativePosition.Transform(new Point(ShellRelativePositonXOffSet, ShellRelativePositonYOffSet));

                if (View is Window window)
                {
                    if (window.WindowState == WindowState.Maximized)
                    {
                        correctedRelativePosition = new Point(correctedRelativePosition.X + MaximizedWindowChromeRelativePositionAdjustment, correctedRelativePosition.Y + MaximizedWindowChromeRelativePositionAdjustment);
                    }
                }

                ExecuteCommand(command, correctedRelativePosition);
            }
        }

        public void OpenWindow(AbstractShellWindow window)
        {
            window.OpenWindow += OpenWindow;
            if (ActiveWindow != null)
            {
                WindowStack.Push(ActiveWindow);
            }

            ActiveWindow = window;
        }

        public override void RequestClose(bool? dialogResult = null)
        {
            if (ProjectManager.CloseProject())
            {
                if (Parent != null)
                {
                    base.RequestClose(dialogResult);
                }
            }
        }

        public void ShowContextMenu(IList<IDiiagramrCommand> commands, Point position)
        {
            ContextMenuViewModel.ShowContextMenu(commands, position);
        }

        public void ShowContextMenu(IList<IDiiagramrCommand> commands)
        {
            ShowContextMenu(commands, new Point(0, 22));
        }

        public void ShowScreen(IScreen screen)
        {
            ActiveItem = screen;
            if (screen is IShownInShellReaction reaction)
            {
                reaction.ShownInShell();
            }
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!ProjectManager.CloseProject())
            {
                e.Cancel = true;
            }
        }

        private string GenerateCommandPath(IDiiagramrCommand command)
        {
            if (command.Parent == null)
            {
                return command.Name;
            }

            return $"{command.Parent}:{command.Name}";
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (ProjectManager.CurrentProject == null)
            {
                ExecuteCommand(StartCommandId);
            }

            CanSaveProject = ProjectManager.CurrentProject != null;
            CanSaveAsProject = ProjectManager.CurrentProject != null;
            WindowTitle = "Diiagramr" + (ProjectManager.CurrentProject != null ? " - " + ProjectManager.CurrentProject.Name : "");
        }

        private void SetupCommands(IEnumerable<IDiiagramrCommand> commands)
        {
            // TODO: Consider making a "CommandHandler" class, that handles this, or make the commands construct themselves this way.
            SetupMenuCommands(commands.OfType<ToolBarCommand>());
            foreach (var command in commands)
            {
                var commandPath = GenerateCommandPath(command);
                if (!_shellCommands.ContainsKey(commandPath))
                {
                    _shellCommands.Add(commandPath, command);
                }
                else
                {
                    if (_shellCommands[commandPath].Weight < command.Weight)
                    {
                        _shellCommands[commandPath] = command;
                    }
                }
            }

            ContextMenuViewModel.ExecuteCommandHandler += ExecuteCommand;
        }

        private void SetupMenuCommands(IEnumerable<IDiiagramrCommand> commands)
        {
            TopLevelMenuItems = new ObservableCollection<TopLevelToolBarCommand>();
            var topLevelMenuItems = commands.OfType<TopLevelToolBarCommand>();
            var nonTopLevelMenuItems = commands.Where(x => x.Parent != null);
            foreach (var topLevelMenuItem in topLevelMenuItems.OrderBy(x => x.Weight))
            {
                TopLevelMenuItems.Add(topLevelMenuItem);
                foreach (var subMenuItem in nonTopLevelMenuItems.Where(x => x.Parent == topLevelMenuItem.Name).OrderBy(x => x.Weight))
                {
                    topLevelMenuItem.SubCommandItems.Add(subMenuItem);
                }
            }
        }
    }
}
