using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows.Media;

namespace VisualDrop
{
    public class LevelThresholdNode : Node
    {
        private int _onThreshold = 1;

        private int _offThreshold = 0;

        private bool _onOffState;

        public LevelThresholdNode()
        {
            Width = 60;
            Height = 60;
            Name = "Threshold";
        }

        public bool InvertOutput { get; set; }

        public int InputValue { get; set; }

        [NodeSetting]
        public int OnThreshold
        {
            get => _onThreshold;

            set
            {
                _onThreshold = value;
                if (OffThreshold > OnThreshold)
                {
                    OffThreshold = OnThreshold;
                }
            }
        }

        [NodeSetting]
        public int OffThreshold
        {
            get => _offThreshold;

            set
            {
                _offThreshold = value;
                if (OnThreshold < OffThreshold)
                {
                    OnThreshold = OffThreshold;
                }
            }
        }

        public Brush ProgressBarForegroundColor { get; set; }

        public float MaxValue { get; private set; }

        [OutputTerminal(Direction.South)]
        public bool Trigger
        {
            get => _onOffState;

            set
            {
                _onOffState = value;
                if (value)
                {
                    ProgressBarForegroundColor = InvertOutput ? Brushes.DarkSlateGray : Brushes.LightSlateGray;
                }
                else
                {
                    ProgressBarForegroundColor = InvertOutput ? Brushes.LightSlateGray : Brushes.DarkSlateGray;
                }
            }
        }

        public void ProgressBarDoubleClicked()
        {
            InvertOutput = !InvertOutput;
            Trigger = !Trigger;
        }

        [InputTerminal(Direction.North)]
        public void LevelInput(int value)
        {
            InputValue = value;
            if (value > MaxValue)
            {
                MaxValue = value;
            }

            if (value < OffThreshold)
            {
                Trigger = InvertOutput;
            }
            else if (value >= OnThreshold)
            {
                Trigger = !InvertOutput;
            }
        }
    }
}