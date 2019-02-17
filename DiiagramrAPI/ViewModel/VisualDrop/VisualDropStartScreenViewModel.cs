using DiiagramrAPI.Service.Interfaces;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media;

namespace DiiagramrAPI.ViewModel.VisualDrop
{
    public class VisualDropStartScreenViewModel : Screen, IShownInShellReaction
    {
        private const int _frames = 45;
        private const int _quadrents = 1;

        private List<Tuple<float, SolidColorBrush>> _targetSpectrumLogoValues = new List<Tuple<float, SolidColorBrush>>();
        private List<List<Tuple<float, SolidColorBrush>>> _logoAnimationFrames = new List<List<Tuple<float, SolidColorBrush>>>();
        public ObservableCollection<Tuple<float, SolidColorBrush>> SpectrumLogoValues { get; set; } = new ObservableCollection<Tuple<float, SolidColorBrush>>();

        public VisualDropStartScreenViewModel()
        {
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53, new SolidColorBrush(Color.FromRgb(188, 47, 51))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27, new SolidColorBrush(Color.FromRgb(188, 47, 51))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(106, new SolidColorBrush(Color.FromRgb(183, 108, 87))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(80, new SolidColorBrush(Color.FromRgb(183, 108, 87))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27, new SolidColorBrush(Color.FromRgb(195, 153, 93))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53, new SolidColorBrush(Color.FromRgb(195, 153, 93))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53, new SolidColorBrush(Color.FromRgb(166, 185, 151))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27, new SolidColorBrush(Color.FromRgb(166, 185, 151))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(80, new SolidColorBrush(Color.FromRgb(98, 147, 104))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(53, new SolidColorBrush(Color.FromRgb(98, 147, 104))));
            _targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(27, new SolidColorBrush(Color.FromRgb(66, 80, 116))));
            //_targetSpectrumLogoValues.Add(new Tuple<float, SolidColorBrush>(80, new SolidColorBrush(Color.FromRgb(66, 80, 116))));

            for (int i = 0; i < _targetSpectrumLogoValues.Count; i++)
            {
                SpectrumLogoValues.Add(_targetSpectrumLogoValues[i]);
            }

            GenerateAnimationFrames();
        }

        private void GenerateAnimationFrames()
        {
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
                    for (int frame = 0; frame < _frames; frame++)
                    {
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
    }
}
