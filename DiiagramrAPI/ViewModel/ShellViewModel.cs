using DiiagramrAPI.Service;
using DiiagramrAPI.Service.Commands;
using DiiagramrAPI.Service.Interfaces;
using DiiagramrAPI.ViewModel.ProjectScreen;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiiagramrAPI.ViewModel
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public const string StartCommandId = "start";
        public Stack<AbstractShellWindow> WindowStack = new Stack<AbstractShellWindow>();
        private Dictionary<string, IDiiagramrCommand> _shellCommands = new Dictionary<string, IDiiagramrCommand>();

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
        public ObservableCollection<IDiiagramrCommand> TopLevelMenuItems { get; set; }
        public string WindowTitle { get; set; } = "Diiagramr";

        public void CloseWindow()
        {
            ActiveWindow.OpenWindow -= OpenWindow;
            ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
        }

        public void ExecuteCommand(IDiiagramrCommand command)
        {
            if (command.CanExecute(this))
            {
                command.Execute(this);
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
                ExecuteCommand(command);
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

        public void ShowContextMenu(IList<IDiiagramrCommand> commands)
        {
            ContextMenuViewModel.Visible = !ContextMenuViewModel.Visible;
            ContextMenuViewModel.Commands.Clear();
            commands.ForEach(ContextMenuViewModel.Commands.Add);
        }

        public void ShowScreen(IScreen screen)
        {
            ActiveItem = screen;
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
            SetupMenuCommands(commands.Where(c => c.ShowInMenu));
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
            TopLevelMenuItems = new ObservableCollection<IDiiagramrCommand>();
            var topLevelMenuItems = commands.Where(x => x.Parent == null);
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
