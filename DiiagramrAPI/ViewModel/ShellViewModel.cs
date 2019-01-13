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
        private Dictionary<string, IDiiagramrCommand> _shellCommands = new Dictionary<string, IDiiagramrCommand>();

        public IProjectManager ProjectManager { get; }
        public LibraryManagerWindowViewModel LibraryManagerWindowViewModel { get; set; }
        public ProjectScreenViewModel ProjectScreenViewModel { get; set; }
        public StartScreenViewModel StartScreenViewModel { get; set; }
        public ContextMenuViewModel ContextMenuViewModel { get; set; }
        public ObservableCollection<IDiiagramrCommand> TopLevelMenuItems { get; set; }

        public bool CanSaveProject { get; set; }
        public bool CanSaveAsProject { get; set; }

        public string WindowTitle { get; set; } = "Diiagramr";

        public Stack<AbstractShellWindow> WindowStack = new Stack<AbstractShellWindow>();
        public AbstractShellWindow ActiveWindow { get; set; }

        public bool IsWindowOpen => ActiveWindow != null;

        public ShellViewModel(
            Func<ProjectScreenViewModel> projectScreenViewModelFactory,
            Func<LibraryManagerWindowViewModel> libraryManagerWindowViewModelFactory,
            Func<StartScreenViewModel> startScreenScreenViewModelFactory,
            Func<IProjectManager> projectManagerFactory,
            Func<IEnumerable<IDiiagramrCommand>> commandsFactory,
            Func<ContextMenuViewModel> contextMenuViewModelFactory)
        {
            ContextMenuViewModel = contextMenuViewModelFactory.Invoke();
            SetupMenuItems(commandsFactory.Invoke());
            ProjectScreenViewModel = projectScreenViewModelFactory.Invoke();
            LibraryManagerWindowViewModel = libraryManagerWindowViewModelFactory.Invoke();
            StartScreenViewModel = startScreenScreenViewModelFactory.Invoke();
            StartScreenViewModel.LoadCanceled += () => ShowScreen(StartScreenViewModel);

            ProjectManager = projectManagerFactory.Invoke();
            ProjectManager.CurrentProjectChanged += ProjectManagerOnCurrentProjectChanged;

            ShowScreen(ProjectScreenViewModel);
            ShowScreen(StartScreenViewModel);
        }

        private void SetupMenuItems(IEnumerable<IDiiagramrCommand> menuItems)
        {
            // TODO: Consider making a "CommandHandler" class, that handles this, or make the commands construct themselves this way.
            TopLevelMenuItems = new ObservableCollection<IDiiagramrCommand>();
            var topLevelMenuItems = menuItems.Where(x => x.Parent == null);
            var nonTopLevelMenuItems = menuItems.Where(x => x.Parent != null);
            foreach (var topLevelMenuItem in topLevelMenuItems.OrderBy(x => x.Weight))
            {
                TopLevelMenuItems.Add(topLevelMenuItem);
                foreach (var subMenuItem in nonTopLevelMenuItems.Where(x => x.Parent == topLevelMenuItem.Name).OrderBy(x => x.Weight))
                {
                    topLevelMenuItem.SubCommandItems.Add(subMenuItem);
                }
            }
            foreach (var command in menuItems)
            {
                var commandPath = GenerateCommandPath(command);
                if (!_shellCommands.ContainsKey(commandPath))
                {
                    _shellCommands.Add(commandPath, command);
                }
            }

            ContextMenuViewModel.ExecuteCommandHandler += ExecuteCommand;
        }

        private string GenerateCommandPath(IDiiagramrCommand command)
        {
            if (command.Parent == null)
            {
                return command.Name;
            }

            return $"{command.Parent}:{command.Name}";
        }

        public void ShowScreen(IScreen screen)
        {
            ActiveItem = screen;
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

        public void CloseWindow()
        {
            ActiveWindow.OpenWindow -= OpenWindow;
            ActiveWindow = WindowStack.Count > 0 ? WindowStack.Pop() : null;
        }

        private void ProjectManagerOnCurrentProjectChanged()
        {
            if (ProjectManager.CurrentProject == null)
            {
                ShowScreen(StartScreenViewModel);
            }

            CanSaveProject = ProjectManager.CurrentProject != null;
            CanSaveAsProject = ProjectManager.CurrentProject != null;
            WindowTitle = "Diiagramr" + (ProjectManager.CurrentProject != null ? " - " + ProjectManager.CurrentProject.Name : "");
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

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!ProjectManager.CloseProject())
            {
                e.Cancel = true;
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

        public void ShowContextMenu(IList<IDiiagramrCommand> commands)
        {
            ContextMenuViewModel.Visible = !ContextMenuViewModel.Visible;
            ContextMenuViewModel.Commands.Clear();
            commands.ForEach(ContextMenuViewModel.Commands.Add);
        }
    }
}
