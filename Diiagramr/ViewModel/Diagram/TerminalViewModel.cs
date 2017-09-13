﻿using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;

namespace Diiagramr.ViewModel.Diagram
{
    public class TerminalViewModel : Screen, IViewAware
    {
        public TerminalModel Terminal { get; set; }

        private readonly IList<Action> _whenViewIsLoadedCallbacks = new List<Action>();
        private Direction _defaultDirection;

        public string Name { get; set; }

        public bool TitleVisible { get; set; }

        public float TerminalRotation { get; set; }

        public virtual object Data { get; set; }

        public double XRelativeToNode
        {
            get { return Terminal.OffsetX; }
            set { Terminal.OffsetX = value; }
        }

        public double YRelativeToNode
        {
            get { return Terminal.OffsetY; }
            set { Terminal.OffsetY = value; }
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            foreach (var whenViewIsLoadedCallback in _whenViewIsLoadedCallbacks)
            {
                whenViewIsLoadedCallback.Invoke();
            }
            _whenViewIsLoadedCallbacks.Clear();
        }

        public Direction DefaultDirection
        {
            get { return _defaultDirection; }
            set
            {
                _defaultDirection = value;
                Terminal.Direction = _defaultDirection;
            }
        }

        public TerminalViewModel(TerminalModel terminal)
        {
            Terminal = terminal;
            terminal.PropertyChanged += terminal.OnTerminalPropertyChanged;
            terminal.PropertyChanged += TerminalOnPropertyChanged;
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
                DragDrop.DoDragDrop(uiElement, dataObjectModel, DragDropEffects.Link);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(uiElement, dataObjectViewModel, DragDropEffects.Link);
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
            var terminal = o as TerminalModel;
            if (terminal == null) return;
            WireToTerminal(terminal);
        }

        public void DisconnectTerminal()
        {
            Terminal.DisconnectWire();
        }

        public virtual void WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null) return;
            if (terminal is InputTerminal && Terminal is OutputTerminal) new Wire((OutputTerminal)Terminal, (InputTerminal)terminal);
            if (Terminal is InputTerminal && terminal is OutputTerminal) new Wire((OutputTerminal)terminal, (InputTerminal)Terminal);
        }

        public virtual void WireFromTerminal(TerminalModel terminal)
        {
            if (terminal == null) return;
            if (terminal is InputTerminal && Terminal is OutputTerminal) new Wire((OutputTerminal)Terminal, (InputTerminal)terminal);
            if (Terminal is InputTerminal && terminal is OutputTerminal) new Wire((OutputTerminal)terminal, (InputTerminal)Terminal);
        }

        public void TerminalMouseDown(object sender, MouseEventArgs e)
        {
            DisconnectTerminal();
            e.Handled = true;
        }

        public void SetTerminalDirection(Direction direction)
        {
            Terminal.Direction = Direction.None;
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
    }
}
