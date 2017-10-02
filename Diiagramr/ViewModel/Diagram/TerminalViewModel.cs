using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Stylet;

namespace Diiagramr.ViewModel.Diagram
{
    public class TerminalViewModel : Screen
    {
        private object _data;

        private Direction _defaultDirection;

        public TerminalViewModel(TerminalModel terminal)
        {
            TerminalModel = terminal ?? throw new ArgumentNullException(nameof(terminal));
            terminal.PropertyChanged += TerminalOnPropertyChanged;
            Data = terminal.Data;
            Name = terminal.Name;
            SetTerminalRotationBasedOnDirection();
        }

        public virtual TerminalModel TerminalModel { get; }

        public string Name { get; set; }

        public virtual bool TitleVisible { get; set; }

        public float TerminalRotation { get; set; }

        public virtual object Data
        {
            get => _data;
            set
            {
                _data = value;
                TerminalModel.Data = value;
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

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(TerminalModel.Direction)))
                SetTerminalRotationBasedOnDirection();
            else if (e.PropertyName.Equals(nameof(Model.TerminalModel.Data)))
                Data = TerminalModel.Data;
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

        public void MouseMove(object sender, MouseEventArgs e)
        {
            var uiElement = (UIElement) sender;
            var dataObjectModel = new DataObject(DataFormats.StringFormat, TerminalModel);
            var dataObjectViewModel = new DataObject(DataFormats.StringFormat, this);
            if (e.LeftButton == MouseButtonState.Pressed)
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    DragDrop.DoDragDrop(uiElement, dataObjectViewModel, DragDropEffects.Link);
                else
                    DragDrop.DoDragDrop(uiElement, dataObjectModel, DragDropEffects.Link);
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
            TerminalModel.DisconnectWire();
        }

        public virtual bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null) return false;
            if (terminal.Kind == TerminalModel.Kind) return false;
            new WireModel(TerminalModel, terminal);
            return true;
        }

        public void TerminalMouseDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        public virtual void SetTerminalDirection(Direction direction)
        {
            TerminalModel.Direction = direction;
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
            TitleVisible = TerminalModel.Type.IsAssignableFrom(type);
        }

        public static TerminalViewModel CreateTerminalViewModel(TerminalModel terminal)
        {
            if (terminal.Kind == TerminalKind.Input) return new InputTerminalViewModel(terminal);
            if (terminal.Kind == TerminalKind.Output) return new OutputTerminalViewModel(terminal);
            return null;
        }
    }
}