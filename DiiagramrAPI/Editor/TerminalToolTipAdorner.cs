﻿using DiiagramrAPI.Editor.Diagrams;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// An adorner to display the name and value on a terminal.
    /// </summary>
    public class TerminalToolTipAdorner : Adorner
    {
        private readonly double marginAroundLabel = 3.0;
        private readonly Border border;
        private readonly TextBlock textBlock;
        private readonly VisualCollection visualChildren;

        /// <summary>
        /// Creates a new instance of <see cref="TerminalToolTipAdorner"/>.
        /// </summary>
        /// <param name="adornedElement">The UI element to adorn.</param>
        /// <param name="adornedTerminal">The terminal whose information to display.</param>
        public TerminalToolTipAdorner(UIElement adornedElement, Terminal adornedTerminal)
            : base(adornedElement)
        {
            if (adornedTerminal == null)
            {
                return;
            }

            AdornedTerminal = adornedTerminal;
            visualChildren = new VisualCollection(this);

            // TODO: make this update when the terminal data changes.
            string text = GetTerminalText();

            textBlock = new TextBlock
            {
                Text = text,
                FontSize = 10,
                Margin = new Thickness(0),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Verdana"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                LineStackingStrategy = LineStackingStrategy.MaxHeight,
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                Foreground = new SolidColorBrush(Color.FromRgb(178, 178, 178)),
            };
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            border = new Border
            {
                Child = textBlock,
                IsHitTestVisible = false,
                Padding = new Thickness(0),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(1),
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                Width = textBlock.DesiredSize.Width + marginAroundLabel * 2,
                MaxWidth = 400,
                Height = textBlock.DesiredSize.Height + marginAroundLabel * 2,
            };
            visualChildren.Add(border);
        }

        /// <summary>
        /// The terminal being adorned.
        /// </summary>
        public Terminal AdornedTerminal { get; set; }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => visualChildren.Count;

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = border.Width;
            double height = border.Height;

            double x = GetRelativeXBasedOnTerminalDirection(width);
            double y = GetRelativeYBasedOnTerminalDirection(height);
            border.Arrange(new Rect(x, y, width, height));
            return finalSize;
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        private string GetTerminalText()
        {
            if (AdornedTerminal.Data == null)
            {
                return AdornedTerminal.Name;
            }
            if (AdornedTerminal.Data.GetType().IsArray)
            {
                var dataArray = AdornedTerminal.Data as Array;
                var arrayText = "";
                for (int i = 0; i < Math.Min(3, dataArray.Length); i++)
                {
                    arrayText += $"{dataArray.GetValue(i)}{((dataArray.Length - 1 != i) ? ", " : "")}";
                }
                return $"{AdornedTerminal.Name} = [{arrayText}{((dataArray.Length > 3) ? " ... " : "")}] Length = {dataArray.Length}";
            }

            return AdornedTerminal.Name + " = " + AdornedTerminal.Data.ToString();
        }

        private double GetRelativeXBasedOnTerminalDirection(double width)
        {
            var direction = AdornedTerminal.TerminalRotation;
            return TerminalAdornerHelpers.GetVisualXBasedOnTerminalDirection(width, direction);
        }

        private double GetRelativeYBasedOnTerminalDirection(double height)
        {
            var direction = AdornedTerminal.TerminalRotation;
            return TerminalAdornerHelpers.GetVisualYBasedOnTerminalDirection(height, direction);
        }
    }
}