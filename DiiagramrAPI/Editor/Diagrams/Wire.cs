using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrAPI.Service.Application;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A wire on the diagram that connects one terminal to another.
    /// </summary>
    public class Wire : ViewModel<WireModel>, IMouseEnterLeaveReaction
    {
        /// <summary>
        /// Whether or not to visually indicate that data is changing across the wire.
        /// </summary>
        public static bool ShowDataPropagation = false;

        private const double _dataVisualDiameter = 6;
        private const float DimWireColorAmount = -0.3f;
        private const float SuperDimWireColorAmount = -0.7f;
        private const int WireAnimationFrameDelay = 20;
        private const int WireAnimationFrames = 15;
        private const int WireDataAnimationFrames = 60;
        private const int WireDataAnimationRepeatDelay = 1000;
        private readonly WirePathingAlgorithum _wirePathingAlgorithum;
        private readonly List<Point> _dataVisualAnimationFrames = new List<Point>();
        private readonly System.Drawing.Color BrokenWireColor = System.Drawing.Color.FromArgb(255, 90, 9, 9);
        private bool _doDataVisualAnimationFramesNeedToBeRefreshed;
        private BackgroundTask _wirePropagationAnimationTask;
        private BackgroundTask _wireCreationAnimationTask;

        /// <summary>
        /// Creates a new instance of <see cref="Wire"/>.
        /// </summary>
        /// <param name="wire">The wire model.</param>
        /// <param name="wirePathingAlgorithum">The algorithum to use to path this wire.</param>
        public Wire(WireModel wire, WirePathingAlgorithum wirePathingAlgorithum)
        {
            _wirePathingAlgorithum = wirePathingAlgorithum;
            WireModel = wire ?? throw new ArgumentNullException(nameof(wire));
            wire.PropertyChanged += ModelPropertyChanged;
            LineColorBrush = GetWireBrush();
        }

        /// <summary>
        /// The visual dash style of the wire.
        /// </summary>
        public DoubleCollection StrokeDashArray { get; set; }

        /// <summary>
        /// The diameter of the circle that move across the wire to show data flow.
        /// </summary>
        public double DataVisualDiameter { get; } = _dataVisualDiameter;

        /// <summary>
        /// The X position of the circle that move across the wire to show data flow.
        /// </summary>
        public double DataVisualX { get; set; }

        /// <summary>
        /// The Y position of the circle that move across the wire to show data flow.
        /// </summary>
        public double DataVisualY { get; set; }

        /// <summary>
        /// Gets or sets whether the circle that move across the wire to show data flow.
        /// </summary>
        public bool IsDataVisualVisible { get; set; }

        /// <summary>
        /// Gets whether the wire is in a broken state.
        /// </summary>
        public bool IsBroken => WireModel?.IsBroken ?? true;

        /// <summary>
        /// Whether to animate the drawing of the wire when the visual is first loaded.
        /// </summary>
        public bool DoAnimationWhenViewIsLoaded { get; set; }

        /// <summary>
        /// The wire color.
        ///
        /// </summary>
        public Brush LineColorBrush { get; set; }

        /// <summary>
        /// The color of the propagation circles.
        /// </summary>
        public Brush PropagationVisualColorBrush { get; set; } = Brushes.Black;

        /// <summary>
        /// The wire model.
        /// </summary>
        public WireModel WireModel { get; private set; }

        /// <summary>
        /// The visible points representing the wireframe of the wire.
        /// </summary>
        public IList<Point> Points { get; private set; }

        /// <summary>
        /// The diagram x coordinate of the start of the wire.
        /// </summary>
        public double X1 => WireModel.X1;

        /// <summary>
        /// The diagram x coordinate of the end of the wire.
        /// </summary>
        public double X2 => WireModel.X2;

        /// <summary>
        /// The diagram y coordinate of the start of the wire.
        /// </summary>
        public double Y1 => WireModel.Y1;

        /// <summary>
        /// The diagram y coordinate of the end of the wire.
        /// </summary>
        public double Y2 => WireModel.Y2;

        private bool CanStartPropagationAnimationTask => ShowDataPropagation && !IsPropagationAnimationTaskRunning;

        private bool IsPropagationAnimationTaskRunning => _wirePropagationAnimationTask != null && _wirePropagationAnimationTask.IsRunning;

        /// <summary>
        /// Disconnects the wire and deletes it.
        /// </summary>
        public void DisconnectWire()
        {
            WireModel.SinkTerminal.DisconnectWire(WireModel, WireModel.SourceTerminal);
        }

        /// <inheritdoc/>
        public void MouseEntered()
        {
            LineColorBrush = GetMouseOverWireBrush();
        }

        /// <inheritdoc/>
        public void MouseLeft()
        {
            LineColorBrush = GetWireBrush();
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(X1)
             || propertyName == nameof(X2)
             || propertyName == nameof(Y1)
             || propertyName == nameof(Y2))
            {
                if (View != null)
                {
                    Points = _wirePathingAlgorithum.GetWirePoints(this);
                    _doDataVisualAnimationFramesNeedToBeRefreshed = true;
                }
            }

            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        protected override void OnViewLoaded()
        {
            if (DoAnimationWhenViewIsLoaded)
            {
                AnimateAndConfigureWirePoints();
            }
            else
            {
                Points = _wirePathingAlgorithum.GetWirePoints(this);
                _doDataVisualAnimationFramesNeedToBeRefreshed = true;
            }
        }

        private static SolidColorBrush GetBrushFromColor(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void AnimateAndConfigureWirePoints()
        {
            if ((_wireCreationAnimationTask == null || !_wireCreationAnimationTask.IsRunning) && WireModel != null)
            {
                _wireCreationAnimationTask = BackgroundTaskManager.Instance.CreateBackgroundTask(AnimateWirePointsOnUiThread).Start();
            }
        }

        private void AnimateDataPropagation(int frameDelay)
        {
            var frames = _dataVisualAnimationFrames.ToList();
            foreach (var dataPointAlongWire in frames)
            {
                View?.Dispatcher.Invoke(() =>
                {
                    DataVisualX = dataPointAlongWire.X;
                    DataVisualY = dataPointAlongWire.Y;
                    IsDataVisualVisible = dataPointAlongWire != frames.Last();
                });
                Thread.Sleep(frameDelay);
            }
        }

        private void AnimateWirePointsOnUiThread()
        {
            var wirePoints = _wirePathingAlgorithum.GetWirePoints(this);
            Array.Reverse(wirePoints);
            var frames = GenerateFramesOfWiringAnimation(wirePoints);
            frames.Add(wirePoints);
            foreach (var frame in frames)
            {
                View?.Dispatcher.Invoke(() => Points = frame);
                Thread.Sleep(WireAnimationFrameDelay);
            }
        }

        private void TryStartPropagationAnimationTask()
        {
            if (CanStartPropagationAnimationTask)
            {
                _wirePropagationAnimationTask = BackgroundTaskManager.Instance.CreateBackgroundTask(DoWirePropagationAnimation).Start();
            }
        }

        private void DoWirePropagationAnimation()
        {
            ValidateDataVisualAnimationFrames();
            AnimateDataPropagation(WireAnimationFrameDelay);
            Thread.Sleep(WireDataAnimationRepeatDelay);
        }

        private void RefreshDataVisualAnimationFrames(IList<Point> originalPoints)
        {
            var _spacedPointsAlongLine = new SpacedPointsAlongLine(originalPoints, _dataVisualDiameter / 2.0, WireDataAnimationFrames);
            _dataVisualAnimationFrames.Clear();
            _dataVisualAnimationFrames.AddRange(_spacedPointsAlongLine.SpacedPoints.Reverse().ToArray());
        }

        private IList<Point[]> GenerateFramesOfWiringAnimation(Point[] originalPoints)
        {
            var _spacedPointsAlongLine = new SpacedPointsAlongLine(originalPoints, constantOffset: 0, WireAnimationFrames, addOriginalPointsToSpacedPoints: true);
            var frames = new List<Point[]>();
            var frame = new List<Point>();
            for (var frameNumber = 0; frameNumber < WireAnimationFrames; frameNumber++)
            {
                frame.Add(_spacedPointsAlongLine.SpacedPoints[frameNumber]);
                frames.Add(frame.ToArray());
            }
            return frames;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Data":
                    TryStartPropagationAnimationTask();
                    break;

                case nameof(WireModel.IsBroken):
                    VisuallyBreakWire();
                    NotifyOfPropertyChange(e.PropertyName);
                    break;

                case nameof(WireModel.X1):
                case nameof(WireModel.X2):
                case nameof(WireModel.Y1):
                case nameof(WireModel.Y2):
                    NotifyOfPropertyChange(e.PropertyName);
                    break;
            }
        }

        private void VisuallyBreakWire()
        {
            StrokeDashArray = new DoubleCollection(new[] { 1.0, 1.0 });
            LineColorBrush = GetWireBrush();
        }

        private System.Drawing.Color GetWireTypeColor()
        {
            var terminalToGetColorFrom = WireModel?.SourceTerminal?.Type == typeof(object)
                            ? WireModel?.SinkTerminal
                            : WireModel?.SourceTerminal;
            return IsBroken
                ? BrokenWireColor
                : TypeColorProvider.Instance.GetColorForType(terminalToGetColorFrom?.CurrentType);
        }

        private Brush GetMouseOverWireBrush()
        {
            return GetBrushFromColor(CoreUilities.ChangeColorBrightness(GetWireTypeColor(), SuperDimWireColorAmount));
        }

        private Brush GetWireBrush()
        {
            return GetBrushFromColor(CoreUilities.ChangeColorBrightness(GetWireTypeColor(), DimWireColorAmount));
        }

        private void ValidateDataVisualAnimationFrames()
        {
            if (_doDataVisualAnimationFramesNeedToBeRefreshed)
            {
                RefreshDataVisualAnimationFrames(Points ?? new List<Point>());
                _doDataVisualAnimationFramesNeedToBeRefreshed = false;
            }
        }
    }
}