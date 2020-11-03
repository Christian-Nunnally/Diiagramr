using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Threading;

namespace VisualDrop.AudioProcessing
{
    public class SpectrumResultDispatcher : ISpectrumResultObserver, ISpectrumResultNotifier
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly ConcurrentQueue<List<float>> _spectrumResultQueue = new ConcurrentQueue<List<float>>();
        private readonly List<ISpectrumResultObserver> _subscribers = new List<ISpectrumResultObserver>();

        public SpectrumResultDispatcher(DispatcherPriority priority)
        {
            _dispatcherTimer = new DispatcherTimer(priority);
            MillisecondsBetweenUpdates = 25;
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
            _dispatcherTimer.IsEnabled = true;
        }

        public int MillisecondsBetweenUpdates
        {
            get => (int)_dispatcherTimer.Interval.TotalMilliseconds;
            set => _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(value);
        }

        public void ObserveSpectrumResults(List<float> spectrum)
        {
            _spectrumResultQueue.Enqueue(spectrum);
        }

        public void Subscribe(ISpectrumResultObserver subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(ISpectrumResultObserver subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            while (_spectrumResultQueue.TryDequeue(out var fftResult))
            {
                _subscribers.ForEach(x => x.ObserveSpectrumResults(fftResult));
            }
        }
    }
}