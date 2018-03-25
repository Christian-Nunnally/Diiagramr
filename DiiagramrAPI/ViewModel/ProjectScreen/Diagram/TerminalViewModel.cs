﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel.Diagram;
using Stylet;

namespace DiiagramrAPI.ViewModel.ProjectScreen.Diagram
{
    public class TerminalViewModel : Screen
    {
        public static TerminalViewModel SelectedTerminal;

        public static CornerRadius TerminalCornerRadius = new CornerRadius(3);
        public static CornerRadius TerminalBorderCornerRadius = new CornerRadius(2);

        public static ColorTheme ColorTheme
        {
            get { return _colorTheme; }
            set
            {
                _colorTheme = value;
                foreach (var action in ActionsToTakeWhenColorThemeIsLoaded)
                {
                    action.Invoke();
                }
            }
        }

        public const double TerminalDiameter = 2 * DiagramViewModel.NodeBorderWidth;

        public static List<Action> ActionsToTakeWhenColorThemeIsLoaded = new List<Action>();
        private static ColorTheme _colorTheme;

        public double TerminalUpWireMinimumLength
        {
            get => TerminalModel.TerminalUpWireMinimumLength;
            set => TerminalModel.TerminalUpWireMinimumLength = value;
        }

        public double TerminalDownWireMinimumLength
        {
            get => TerminalModel.TerminalDownWireMinimumLength;
            set => TerminalModel.TerminalDownWireMinimumLength = value;
        }

        public double TerminalLeftWireMinimumLength
        {
            get => TerminalModel.TerminalLeftWireMinimumLength;
            set => TerminalModel.TerminalLeftWireMinimumLength = value;
        }

        public double TerminalRightWireMinimumLength
        {
            get => TerminalModel.TerminalRightWireMinimumLength;
            set => TerminalModel.TerminalRightWireMinimumLength = value;
        }

        public Brush TerminalBackgroundBrush { get; set; }

        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                WiringModeChanged?.Invoke(this, _isSelected);

                if (SelectedTerminal == this)
                {
                    SelectedTerminal = null;
                    _isSelected = false;
                    return;
                }

                if (SelectedTerminal != null)
                {
                    if (SelectedTerminal.WireToTerminal(TerminalModel))
                    {
                        _isSelected = false;
                        return;
                    }
                }

                if (SelectedTerminal != null) SelectedTerminal.IsSelected = false;
                SelectedTerminal = _isSelected ? this : null;
            }
        }

        private object _data;

        public TerminalViewModel(TerminalModel terminal)
        {
            TerminalModel = terminal ?? throw new ArgumentNullException(nameof(terminal));
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            Data = terminal.Data;
            Name = terminal.Name;
            SetTerminalRotationBasedOnDirection();

            if (ColorTheme != null)
            {
                TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(terminal.Type));
            }
            else
            {
                ActionsToTakeWhenColorThemeIsLoaded.Add(() =>
                {
                    TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(terminal.Type));
                });
            }
        }

        public virtual TerminalModel TerminalModel { get; }

        public Action<object> DataChanged;
        public event Action<TerminalViewModel, bool> WiringModeChanged;

        private bool _isSelected;

        public string Name { get; set; }

        public virtual bool MouseWithin { get; set; }
        public virtual bool TitleVisible => MouseWithin || IsSelected;
        public virtual bool HighlightVisible { get; set; }

        public float TerminalRotation { get; set; }

        public virtual object Data
        {
            get => _data;
            set
            {
                _data = value;
                TerminalModel.Data = value;
                DataChanged?.Invoke(Data);
            }
        }

        public double XRelativeToNode
        {
            get => TerminalModel.OffsetX;
            set => TerminalModel.OffsetX = value;
        }

        public double YRelativeToNode
        {
            get => TerminalModel.OffsetY;
            set => TerminalModel.OffsetY = value;
        }

        public int EdgeIndex
        {
            get => TerminalModel.EdgeIndex;
            set => TerminalModel.EdgeIndex = value;
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.Direction)))
                SetTerminalRotationBasedOnDirection();
            else if (e.PropertyName.Equals(nameof(Model.TerminalModel.Data)))
            {
                Data = TerminalModel.Data;
            }
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            switch (TerminalModel.Direction)
            {
                case Direction.North:
                    TerminalRotation = 0;
                    break;
                case Direction.East:
                    TerminalRotation = 90;
                    break;
                case Direction.South:
                    TerminalRotation = 180;
                    break;
                default:
                    TerminalRotation = 270;
                    break;
            }
        }

        public void CalculateUTurnLimitForTerminalSoThatWiresAvoidTheNodes(double nodeWidth, double nodeHeight)
        {
            const double marginFromEdgeOfNode = DiagramViewModel.NodeBorderWidth + 10;
            var offsetX = TerminalModel.OffsetX;
            var offsetY = TerminalModel.OffsetY;
            var terminalDirection = TerminalModel.Direction;

            if (terminalDirection == Direction.North)
            {
                TerminalUpWireMinimumLength = 0;
                TerminalDownWireMinimumLength = nodeHeight + marginFromEdgeOfNode * 2;
                TerminalLeftWireMinimumLength = offsetX + marginFromEdgeOfNode;
                TerminalRightWireMinimumLength = nodeWidth - offsetX + marginFromEdgeOfNode;
            }
            else if (terminalDirection == Direction.South)
            {
                TerminalUpWireMinimumLength = nodeHeight + marginFromEdgeOfNode * 2;
                TerminalDownWireMinimumLength = 0;
                TerminalLeftWireMinimumLength = offsetX + marginFromEdgeOfNode;
                TerminalRightWireMinimumLength = nodeWidth - offsetX + marginFromEdgeOfNode;
            }
            else if (terminalDirection == Direction.East)
            {
                TerminalUpWireMinimumLength = offsetY + marginFromEdgeOfNode;
                TerminalDownWireMinimumLength = nodeHeight - offsetY + marginFromEdgeOfNode;
                TerminalLeftWireMinimumLength = nodeWidth + marginFromEdgeOfNode * 2;
                TerminalRightWireMinimumLength = 0;
            }
            else if (terminalDirection == Direction.West)
            {
                TerminalUpWireMinimumLength = offsetY + marginFromEdgeOfNode;
                TerminalDownWireMinimumLength = nodeHeight - offsetY + marginFromEdgeOfNode;
                TerminalLeftWireMinimumLength = 0;
                TerminalRightWireMinimumLength = nodeWidth + marginFromEdgeOfNode * 2;
            }
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
//            var uiElement = (UIElement) sender;
//            var dataObjectModel = new DataObject(DataFormats.StringFormat, TerminalModel);
//            var dataObjectViewModel = new DataObject(DataFormats.StringFormat, this);
//            if (e.LeftButton == MouseButtonState.Pressed)
//                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
//                    DragDrop.DoDragDrop(uiElement, dataObjectViewModel, DragDropEffects.Link);
//                else
//                    DragDrop.DoDragDrop(uiElement, dataObjectModel, DragDropEffects.Link);
            e.Handled = true;
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            DropObject(o);
        }

        public virtual void DropObject(object o)
        {
            if (!(o is TerminalModel terminal)) return;
            WireToTerminal(terminal);
        }

        public virtual void DisconnectTerminal()
        {
            TerminalModel.DisconnectWires();
        }

        public virtual bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null) return false;
            if (terminal.Kind == TerminalModel.Kind) return false;
            if (terminal.ConnectedWires.Intersect(TerminalModel.ConnectedWires).Any())
            {
                return false;
            }
            new WireModel(TerminalModel, terminal);
            return true;
        }

        public void TerminalLeftMouseDownHandler(object sender, MouseEventArgs e)
        {
            TerminalLeftMouseDown();
            e.Handled = true;
        }

        public void TerminalLeftMouseDown()
        {
            IsSelected = true;
        }

        public virtual void SetTerminalDirection(Direction direction)
        {
            TerminalModel.Direction = direction;
        }

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            MouseWithin = true;
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            MouseWithin = false;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            HighlightVisible = TerminalModel.Type.IsAssignableFrom(type);
        }

        public static TerminalViewModel CreateTerminalViewModel(TerminalModel terminal)
        {
            if (terminal.Kind == TerminalKind.Input) return new InputTerminalViewModel(terminal);
            return new OutputTerminalViewModel(terminal);
        }
    }
}