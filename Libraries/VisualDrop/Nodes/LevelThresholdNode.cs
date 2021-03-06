using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System.Windows.Media;

namespace VisualDrop
{
    public class LevelThresholdNode : Node
    {
        private float _onThreshold = 1;
        private float _offThreshold = 0;
        private bool _onOffState;
        private long _ticksOn = 1;
        private long _ticksOff = 1;
        private float dutyCycleTarget = .5f;

        public LevelThresholdNode()
        {
            Width = 60;
            Height = 60;
            Name = "Threshold";
        }

        public bool InvertOutput { get; set; }

        public float InputValue { get; set; }

        [NodeSetting]
        public float OnThreshold
        {
            get => _onThreshold;

            set
            {
                if (value >= 0 && value <= 1)
                {
                    _onThreshold = value;
                    if (OffThreshold > OnThreshold)
                    {
                        OffThreshold = OnThreshold;
                    }
                }
            }
        }

        [NodeSetting]
        public float OffThreshold
        {
            get => _offThreshold;

            set
            {
                if (value >= 0 && value <= 1)
                {
                    _offThreshold = value;
                    if (OnThreshold < OffThreshold)
                    {
                        OnThreshold = OffThreshold;
                    }
                }
            }
        }

        public Brush ProgressBarForegroundColor { get; set; } = Brushes.Purple;

        public float MaxValue { get; private set; }

        [OutputTerminal(Direction.South)]
        public bool Trigger
        {
            get => _onOffState;

            set
            {
                if (value != _onOffState)
                {
                    float dutyCycle = (float)_ticksOn / (_ticksOn + _ticksOff);
                    if (dutyCycle < DutyCycleTarget)
                    {
                        if (OnThreshold > 0.005f)
                        {
                            OnThreshold -= 0.01f;
                            OnPropertyChanged(nameof(OnThreshold));
                        }
                        else
                        {
                            OffThreshold += 0.01f;
                            OnPropertyChanged(nameof(OffThreshold));
                        }
                    }
                    else
                    {
                        if (OnThreshold < 0.995f)
                        {
                            OnThreshold += 0.01f;
                            OnPropertyChanged(nameof(OnThreshold));
                        }
                        else
                        {
                            OffThreshold += 0.01f;
                            OnPropertyChanged(nameof(OffThreshold));
                        }
                    }

                    //ProgressBarForegroundColor = value
                    //    ? InvertOutput ? Brushes.DarkSlateGray : Brushes.LightSlateGray
                    //    : InvertOutput ? Brushes.LightSlateGray : Brushes.DarkSlateGray;
                    _onOffState = value;
                }
            }
        }

        [InputTerminal(Direction.North)]
        public float LevelInput
        {
            set
            {
                InputValue = value;

                if (value < OffThreshold)
                {
                    Trigger = InvertOutput;
                }
                else if (value >= OnThreshold)
                {
                    Trigger = !InvertOutput;
                }
                if (Trigger) _ticksOn++;
                if (!Trigger) _ticksOff++;
            }
            get => InputValue;
        }

        [InputTerminal(Direction.West)]
        public float DutyCycleTarget
        {
            get => dutyCycleTarget;
            set
            {
                _ticksOff = 1;
                _ticksOff = 1;
                dutyCycleTarget = value;
            }
        }

        [InputTerminal(Direction.West)]
        public float SwitchFrequencyTarget { get; set; }

        public void ProgressBarDoubleClicked()
        {
            InvertOutput = !InvertOutput;
            Trigger = !Trigger;
        }
    }
}