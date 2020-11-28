using System;
using System.Collections.Concurrent;
using System.Windows.Threading;

namespace VisualDrop
{
    public class ArrayResultDispatcher : IArrayConsumer, IArrayProducer
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly ConcurrentQueue<float[]> _arrayQueue = new ConcurrentQueue<float[]>();

        public ArrayResultDispatcher(DispatcherPriority priority, int millisecondsBetweenDispatches = 25)
        {
            _dispatcherTimer = new DispatcherTimer(priority);
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(millisecondsBetweenDispatches);
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
            _dispatcherTimer.IsEnabled = true;
        }

        public IArrayConsumer Consumer { get; set; }

        public void ConsumeArray(float[] array)
        {
            _arrayQueue.Enqueue(array);
        }

        private void OnDispatcherTimerTick(object sender, EventArgs e)
        {
            while (_arrayQueue.TryDequeue(out var array))
            {
                Consumer.ConsumeArray(array);
            }
        }
    }
}