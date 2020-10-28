using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Threading;

namespace VisualDrop.AudioProcessing
{
    public class FFTResultUIDispatcher : ISpectrumResultObserver, ISpectrumResultNotifier
    {
        private readonly DispatcherTimer _audioDataReceivedTimer = new DispatcherTimer(DispatcherPriority.Render);
        private readonly ConcurrentQueue<List<float>> _fftResultQueue = new ConcurrentQueue<List<float>>();
        private readonly List<ISpectrumResultObserver> _subscribers = new List<ISpectrumResultObserver>();

        public FFTResultUIDispatcher()
        {
            MillisecondsBetweenUpdates = 33;
            _audioDataReceivedTimer.Tick += OnAudioDataReceivedTimerTick;
            _audioDataReceivedTimer.IsEnabled = true;
        }

        public event Action<List<float>> FftDataReceived;

        public int MillisecondsBetweenUpdates
        {
            get => (int)_audioDataReceivedTimer.Interval.TotalMilliseconds;
            set => _audioDataReceivedTimer.Interval = TimeSpan.FromMilliseconds(value);
        }

        public void ObserveSpectrumResults(List<float> fftResults)
        {
            _fftResultQueue.Enqueue(fftResults);
        }

        public void Subscribe(ISpectrumResultObserver subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(ISpectrumResultObserver subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        private void OnAudioDataReceivedTimerTick(object sender, EventArgs e)
        {
            while (_fftResultQueue.TryDequeue(out var fftResult))
            {
                _subscribers.ForEach(x => x.ObserveSpectrumResults(fftResult));
            }
        }
    }
}