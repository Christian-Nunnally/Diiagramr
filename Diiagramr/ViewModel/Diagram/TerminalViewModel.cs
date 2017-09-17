﻿using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class TerminalViewModel : Screen, IViewAware
    {
        public TerminalModel Terminal { get; private set; }

        private Direction _defaultDirection;
        private object _data;

        public string Name { get; set; }

        public bool TitleVisible { get; set; }

        public float TerminalRotation { get; set; }

        public virtual object Data
        {
            get => _data;
            set
            {
                _data = value;
                Terminal.Data = value;
            }
        }

        public double XRelativeToNode
        {
            get => Terminal.OffsetX;
            set => Terminal.OffsetX = value;
        }

        public double YRelativeToNode
        {
            get => Terminal.OffsetY;
            set => Terminal.OffsetY = value;
        }

        public Direction DefaultDirection
        {
            get => _defaultDirection;
            set
            {
                _defaultDirection = value;
                Terminal.Direction = _defaultDirection;
            }
        }

        public TerminalViewModel(TerminalModel terminal)
        {
            Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            terminal.PropertyChanged += terminal.OnTerminalPropertyChanged;
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            DefaultDirection = terminal.Direction;
            Name = Terminal.Name;
            SetTerminalRotationBasedOnDirection();
            TitleVisible = true;
            TitleVisible = false;
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Terminal.Direction)))
            {
                SetTerminalRotationBasedOnDirection();
            }
            else if (e.PropertyName.Equals(nameof(TerminalModel.Data)))
            {
                Data = Terminal.Data;
            }
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            switch (Terminal.Direction)
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

        public void MouseMove(object sender, MouseEventArgs e)
        {
            var uiElement = (UIElement)sender;
            var dataObjectModel = new DataObject(DataFormats.StringFormat, Terminal);
            var dataObjectViewModel = new DataObject(DataFormats.StringFormat, this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    DragDrop.DoDragDrop(uiElement, dataObjectViewModel, DragDropEffects.Link);
                }
                else
                {
                    DragDrop.DoDragDrop(uiElement, dataObjectModel, DragDropEffects.Link);
                }
            }
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

        public void DisconnectTerminal()
        {
            Terminal.DisconnectWire();
        }

        public virtual bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null) return false;
            if (terminal.Kind == Terminal.Kind) return false;
            new WireModel(Terminal, terminal);
            return true;
        }

        public void TerminalMouseDown(object sender, MouseEventArgs e)
        {
            DisconnectTerminal();
            e.Handled = true;
        }

        public void SetTerminalDirection(Direction direction)
        {;
            Terminal.Direction = direction;
        }

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            TitleVisible = true;
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            TitleVisible = false;
        }

        public void ShowLabelIfCompatibleType(Type type)
        {
            TitleVisible = Terminal.Type.IsAssignableFrom(type);
        }

        public static TerminalViewModel CreateTerminalViewModel(TerminalModel terminal)
        {
            if (terminal.Kind == TerminalKind.Input) return new InputTerminalViewModel(terminal);
            if (terminal.Kind == TerminalKind.Output) return new OutputTerminalViewModel(terminal);
            return null;
        }
    }
}
