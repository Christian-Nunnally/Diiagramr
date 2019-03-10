using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media;

namespace DiiagramrAPI.Shell
{
    public class VisualDropStartScreenViewModel : Screen, IShownInShellReaction
    {
        private const int _recentProjectMaxCharacterLength = 10;
        private const int _dripEffectDelay = 30;
        private const int _frames = 100;
        private const int _quadrents = 1;

        private readonly List<Tuple<float, SolidColorBrush>> _targetSpectrumLogoValues = new List<Tuple<float, SolidColorBrush>>();
        private readonly List<List<Tuple<float, SolidColorBrush>>> _logoAnimationFrames = new List<List<Tuple<float, SolidColorBrush>>>();
        private IProjectFileService _projectFileService;

        public ObservableCollection<Tuple<float, SolidColorBrush>> SpectrumLogoValues { get; set; } = new ObservableCollection<Tuple<float, SolidColorBrush>>();

        public bool OpenProjectButtonsVisible { get; set; }
        public bool OpenProjectLabelVisible => !OpenProjectButtonsVisible;
        public string RecentProject1 { get; set; }
        public string RecentProject2 { get; set; }
        public string RecentProject3 { get; set; }

        public string RecentProject1DisplayString => RecentProject1.Length > _recentProjectMaxCharacterLength
            ? RecentProject1.Substring(0, _recentProjectMaxCharacterLength).Trim() + "..."
            : RecentProject1;
        public string RecentProject2DisplayString => RecentProject2.Length > _recentProjectMaxCharacterLength
            ? RecentProject2.Substring(0, _recentProjectMaxCharacterLength).Trim() + "..."
            : RecentProject2;
        public string RecentProject3DisplayString => RecentProject3.Length > _recentProjectMaxCharacterLength
            ? RecentProject3.Substring(0, _recentProjectMaxCharacterLength).Trim() + "..."
            : RecentProject3;

        public VisualDropStartScreenViewModel(Func<IProjectFileService> projectFileServiceFactory)
        {
            _projectFileService = projectFileServiceFactory.Invoke();
            _projectFileService.ProjectSaved += ProjectSavedHandler;
            PopulateTargetSpectrumValues();

            for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
            {
                SpectrumLogoValues.Add(_targetSpectrumLogoValues[i]);
            }

            GenerateAnimationFrames();

            RecentProject1 = Properties.Settings.Default.RecentProject1;
            RecentProject2 = Properties.Settings.Default.RecentProject2;
            RecentProject3 = Properties.Settings.Default.RecentProject3;
            RecentProject1 = string.IsNullOrWhiteSpace(RecentProject1) ? "Recent #1" : RecentProject1;
            RecentProject2 = string.IsNullOrWhiteSpace(RecentProject2) ? "Recent #2" : RecentProject2;
            RecentProject3 = string.IsNullOrWhiteSpace(RecentProject3) ? "Recent #3" : RecentProject3;
            UpdateRecentProjects(string.Empty);
        }

        private void PopulateTargetSpectrumValues()
        {
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53 / 2, new SolidColorBrush(Color.FromRgb(188, 47, 51))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(46, new SolidColorBrush(Color.FromRgb(183, 108, 87))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27 / 2, new SolidColorBrush(Color.FromRgb(195, 153, 93))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53 / 2, new SolidColorBrush(Color.FromRgb(166, 185, 151))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(38, new SolidColorBrush(Color.FromRgb(98, 147, 104))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27 / 2, new SolidColorBrush(Color.FromRgb(66, 80, 116))));
        }

        private void ProjectSavedHandler(ProjectModel project)
        {
            UpdateRecentProjects(project.Name);
        }

        private void GenerateAnimationFrames()
        {
            _logoAnimationFrames.Clear();
            var tempAnimationFrames = new List<List<Tuple<float, SolidColorBrush>>>();
            for (int frame = 0; frame < _frames; frame++)
            {
                var currentFrame = new List<Tuple<float, SolidColorBrush>>();
                for (int j = 0; j < _targetSpectrumLogoValues.Count; j++)
                {
                    var targetAmplitute = _targetSpectrumLogoValues[j].Item1;
                    var targetColor = _targetSpectrumLogoValues[j].Item2;
                    var d = _quadrents * ((Math.PI / 2.0) / _frames) * frame;
                    var frameAmplitutde = (float)(targetAmplitute * Math.Sin(d));

                    currentFrame.Add(new Tuple<float, SolidColorBrush>(frameAmplitutde, targetColor));
                }

                tempAnimationFrames.Add(currentFrame);
            }

            var random = new Random();
            var _offsets = new List<int>();
            for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
            {
                _offsets.Add(random.Next(_dripEffectDelay - 2));
            }

            for (int frame = 0; frame < _frames + _dripEffectDelay; frame++)
            {
                var currentFrame = new List<Tuple<float, SolidColorBrush>>();
                for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
                {
                    var dripEffectIndex = frame > _offsets[i]
                        ? frame - _offsets[i]
                        : 0;
                    dripEffectIndex = dripEffectIndex >= tempAnimationFrames.Count
                        ? tempAnimationFrames.Count - 1
                        : dripEffectIndex;
                    var frameAplitute = tempAnimationFrames[dripEffectIndex][i].Item1;
                    var frameColor = tempAnimationFrames[dripEffectIndex][i].Item2;
                    currentFrame.Add(new Tuple<float, SolidColorBrush>(frameAplitute, frameColor));
                }

                _logoAnimationFrames.Add(currentFrame);
            }
        }

        public void ShownInShell()
        {
            AnimateLogo();
        }

        private void AnimateLogo()
        {
            if (View != null)
            {
                new Thread(() =>
                {
                    for (int frame = 0; frame < _frames + _dripEffectDelay; frame++)
                    {
                        if (View == null)
                        {
                            return;
                        }

                        View.Dispatcher.Invoke(() =>
                        {
                            for (int j = 0; j < _targetSpectrumLogoValues.Count; j++)
                            {
                                SpectrumLogoValues[j] = _logoAnimationFrames[frame][j];
                            }
                        });
                        Thread.Sleep(14);
                    }
                }).Start();
            }
        }

        protected override void OnViewLoaded()
        {
            AnimateLogo();
        }

        public void NewButtonPressed()
        {
            ShellCommand.Execute("Project:New");
        }

        public void BrowseButtonPressed()
        {
            ShellCommand.Execute("Project:Open");
        }

        private void LoadProject(string projectName)
        {
            ShellCommand.Execute("Project:Open", projectName);
        }

        public void UpdateRecentProjects(string name)
        {
            if (name == Properties.Settings.Default.RecentProject1
             || name == Properties.Settings.Default.RecentProject2
             || name == Properties.Settings.Default.RecentProject3)
            {
                return;
            }
            if (!string.IsNullOrEmpty(name))
            {
                Properties.Settings.Default.RecentProject3 = Properties.Settings.Default.RecentProject2;
                Properties.Settings.Default.RecentProject2 = Properties.Settings.Default.RecentProject1;
                Properties.Settings.Default.RecentProject1 = name;
                Properties.Settings.Default.Save();
            }
            RecentProject1 = Properties.Settings.Default.RecentProject1;
            RecentProject2 = Properties.Settings.Default.RecentProject2;
            RecentProject3 = Properties.Settings.Default.RecentProject3;
        }

        public void OpenLabelMouseEntered()
        {
            OpenProjectButtonsVisible = true;
        }

        public void OpenButtonsMouseLeave()
        {
            OpenProjectButtonsVisible = false;
        }

        public void RecentProject1Pressed()
        {
            if (RecentProject1 == "Recent #1")
            {
                BrowseButtonPressed();
            }
            else
            {
                LoadProject(RecentProject1);
            }
        }

        public void RecentProject2Pressed()
        {
            if (RecentProject2 == "Recent #2")
            {
                BrowseButtonPressed();
            }
            else
            {
                LoadProject(RecentProject2);
            }
        }

        public void RecentProject3Pressed()
        {
            if (RecentProject3 == "Recent #3")
            {
                BrowseButtonPressed();
            }
            else
            {
                LoadProject(RecentProject3);
            }
        }
    }
}
