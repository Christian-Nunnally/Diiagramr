using DiiagramrAPI.Application.ShellCommands.FileCommands;
using DiiagramrAPI.Service.Application;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Media;

namespace DiiagramrAPI.Application
{
    /// <summary>
    /// The visual drop themed start screen.
    /// </summary>
    public class VisualDropStartScreen : Screen, IShownInShellReaction
    {
        private const int _dripEffectDelay = 30;
        private const int _frames = 100;
        private const int _quadrents = 1;
        private readonly List<List<Tuple<float, SolidColorBrush>>> _logoAnimationFrames = new List<List<Tuple<float, SolidColorBrush>>>();
        private readonly List<Tuple<float, SolidColorBrush>> _targetSpectrumLogoValues = new List<Tuple<float, SolidColorBrush>>();
        private readonly OpenProjectCommand _openProjectCommand;
        private readonly NewProjectCommand _newProjectCommand;

        /// <summary>
        /// Creates a new instance of <see cref="VisualDropStartScreen"/>.
        /// </summary>
        /// <param name="openProjectCommandFactory">A factory that provides an instance of <see cref="OpenProjectCommand"/>.</param>
        /// <param name="newProjectCommandFactory">A factory that provides an instance of <see cref="NewProjectCommand"/>.</param>
        public VisualDropStartScreen(
            Func<OpenProjectCommand> openProjectCommandFactory,
            Func<NewProjectCommand> newProjectCommandFactory)
        {
            _openProjectCommand = openProjectCommandFactory();
            _newProjectCommand = newProjectCommandFactory();
            PopulateTargetSpectrumValues();

            for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
            {
                SpectrumLogoValues.Add(_targetSpectrumLogoValues[i]);
            }

            GenerateAnimationFrames();
        }

        /// <summary>
        /// Whether or not the open project button is visible.
        /// </summary>
        public bool OpenProjectButtonsVisible { get; set; }

        /// <summary>
        /// Gets wheter the open project label is visible.
        /// </summary>
        public bool OpenProjectLabelVisible => !OpenProjectButtonsVisible;

        /// <summary>
        /// A list of values and colors for the drippy rainbow in the 'O' of the "Visual Drop' logo on the start screen.
        /// </summary>
        public ObservableCollection<Tuple<float, SolidColorBrush>> SpectrumLogoValues { get; } = new ObservableCollection<Tuple<float, SolidColorBrush>>();

        /// <summary>
        /// Occurs when the browse projects button is pressed.
        /// </summary>
        public void BrowseButtonPressed()
        {
            _openProjectCommand.Execute();
        }

        /// <summary>
        /// Occurs when the browse projects button is pressed.
        /// </summary>
        public void TemplateButtonPressed()
        {
            _openProjectCommand.Execute();
        }

        /// <summary>
        /// Occurs when the new project button is pressed.
        /// </summary>
        public void NewButtonPressed()
        {
            _newProjectCommand.Execute();
        }

        /// <summary>
        /// Occurs when the mouse leaves the open projects area.
        /// </summary>
        public void OpenButtonsMouseLeave()
        {
            OpenProjectButtonsVisible = false;
        }

        /// <summary>
        /// Occurs when the mouse enters the open projects area.
        /// </summary>
        public void OpenLabelMouseEntered()
        {
            OpenProjectButtonsVisible = true;
        }

        /// <inheritdoc/>
        public void ShownInShell()
        {
            AnimateLogo();
        }

        /// <inheritdoc/>
        protected override void OnViewLoaded()
        {
            AnimateLogo();
        }

        private static float CalculateFrameAmplitude(int frameNumber, float targetAmplitute)
        {
            var frameRadians = _quadrents * (Math.PI / 2.0 / _frames) * frameNumber;
            return (float)(targetAmplitute * Math.Sin(frameRadians));
        }

        private void AnimateLogo()
        {
            if (View != null)
            {
                new Thread(() =>
                {
                    for (int frame = 0; frame < _frames + _dripEffectDelay; frame++)
                    {
                        View?.Dispatcher.Invoke(() =>
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

        private void GenerateAnimationFrames()
        {
            _logoAnimationFrames.Clear();

            var random = new Random();
            var offsets = new List<int>();
            for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
            {
                offsets.Add(random.Next(_dripEffectDelay - 2));
            }

            var basicAnimationFrames = GenerateBasicAnimationForLogoSpectrum();
            for (int frame = 0; frame < _frames + _dripEffectDelay; frame++)
            {
                var currentFrame = new List<Tuple<float, SolidColorBrush>>();
                for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
                {
                    var offsetFrameIndex = frame - offsets[i];
                    offsetFrameIndex = Math.Min(basicAnimationFrames.Count - 1, Math.Max(0, offsetFrameIndex));
                    var frameAplitute = basicAnimationFrames[offsetFrameIndex][i].Item1;
                    var frameColor = basicAnimationFrames[offsetFrameIndex][i].Item2;
                    currentFrame.Add(new Tuple<float, SolidColorBrush>(frameAplitute, frameColor));
                }

                _logoAnimationFrames.Add(currentFrame);
            }
        }

        private List<List<Tuple<float, SolidColorBrush>>> GenerateBasicAnimationForLogoSpectrum()
            => Enumerable.Range(0, _frames).Select(GenerateSingleFrameOfAnimationForLogoSpectrum).ToList();

        private List<Tuple<float, SolidColorBrush>> GenerateSingleFrameOfAnimationForLogoSpectrum(int frameNumber)
            => Enumerable.Range(0, _targetSpectrumLogoValues.Count).Select(i => GenerateSpectrumValueColorPairForFrame(frameNumber, i)).ToList();

        private Tuple<float, SolidColorBrush> GenerateSpectrumValueColorPairForFrame(int frameNumber, int spectrumIndex)
        {
            var targetAmplitute = _targetSpectrumLogoValues[spectrumIndex].Item1;
            var targetColor = _targetSpectrumLogoValues[spectrumIndex].Item2;
            var frameAmplitutde = CalculateFrameAmplitude(frameNumber, targetAmplitute);
            return new Tuple<float, SolidColorBrush>(frameAmplitutde, targetColor);
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
    }
}