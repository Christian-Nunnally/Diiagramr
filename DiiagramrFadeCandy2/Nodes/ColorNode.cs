using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DiiagramrFadeCandy
{
    public class ColorNode : Node
    {
        public ColorNode()
        {
            Name = "Color Picker";
            Width = 60;
            Height = 60;

            ColorWheelBitmapImage = new BitmapImage(new Uri("pack://application:,,,/DiiagramrFadeCandy2;component/Resources/colorpicker.png"));

            ColorOutput.R = SelectedColorBrush.Color.R / 255f;
            ColorOutput.G = SelectedColorBrush.Color.G / 255f;
            ColorOutput.B = SelectedColorBrush.Color.B / 255f;
            ColorOutput.A = SelectedColorBrush.Color.A / 255f;
        }

        [OutputTerminal(Direction.South)]
        public Color ColorOutput { get; set; } = new Color();

        public Bitmap ColorWheelBitmap { get; set; }

        public BitmapImage ColorWheelBitmapImage { get; set; }

        public SolidColorBrush SelectedColorBrush { get; set; } = new SolidColorBrush(new System.Windows.Media.Color() { R = 50, G = 100, B = 65, A = 255 });

        public float ColorPaletteImageMargin { get; } = 3;

        public bool IsColorPickerVisible { get; set; }

        [InputTerminal(Direction.East)]
        public void PickRandomColor(bool data)
        {
            if (data)
            {
                var random = new Random();
                var randomBytes = new byte[3];
                random.NextBytes(randomBytes);
                float floatR;
                float floatG;
                float floatB;
                if (random.Next(2) > 0)
                {
                    if (random.Next(2) > 0)
                    {
                        floatR = 1.0f / 255.0f * randomBytes[0];
                        floatG = 0.3f / 255.0f * randomBytes[1];
                        floatB = 0.3f / 255.0f * randomBytes[2];
                    }
                    else
                    {
                        floatR = 0.3f / 255.0f * randomBytes[0];
                        floatG = 0.3f / 255.0f * randomBytes[1];
                        floatB = 1.0f / 255.0f * randomBytes[2];
                    }
                }
                else
                {
                    if (random.Next(2) > 0)
                    {
                        floatR = 0.3f / 255.0f * randomBytes[0];
                        floatG = 1.0f / 255.0f * randomBytes[1];
                        floatB = 0.3f / 255.0f * randomBytes[2];
                    }
                    else
                    {
                        floatR = 0.3f / 255.0f * randomBytes[0];
                        floatG = 0.3f / 255.0f * randomBytes[1];
                        floatB = 0.3f / 255.0f * randomBytes[2];
                    }
                }
                SetColorOnTerminal(floatR, floatG, floatB, 1.0f);
            }
        }

        public void ColorWheelMouseDown(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            System.Windows.Point position;
            if (frameworkElement != null && frameworkElement.IsMouseOver)
            {
                position = e.GetPosition(frameworkElement);
            }
            else
            {
                position = new System.Windows.Point(-3, -3);
            }

            if (Width == double.NaN || Width == 0)
            {
                return;
            }

            if (Height == double.NaN || Height == 0)
            {
                return;
            }

            var paletteImageWidth = Width - 2 * ColorPaletteImageMargin;
            var paletteImageHeight = Height - 2 * ColorPaletteImageMargin;
            var xRelativeToBitmap = ColorWheelBitmap.Width / paletteImageWidth * position.X;
            var yRelativeToBitmap = ColorWheelBitmap.Height / paletteImageHeight * position.Y;

            var color = ColorWheelBitmap.GetPixel((int)xRelativeToBitmap, (int)yRelativeToBitmap);

            var floatR = 1.0f / 255.0f * color.R;
            var floatG = 1.0f / 255.0f * color.G;
            var floatB = 1.0f / 255.0f * color.B;
            var floatA = 1.0f / 255.0f * color.A;
            SetColorOnTerminal(floatR, floatG, floatB, floatA);
        }

        [InputTerminal(Direction.North)]
        public void RedInputTerminal_DataChanged(float data)
        {
            if (ColorOutput != null)
            {
                SetColorOnTerminal(data, ColorOutput.G, ColorOutput.B, ColorOutput.A);
            }
        }

        [InputTerminal(Direction.North)]
        public void GreenInputTerminal_DataChanged(float data)
        {
            if (ColorOutput != null)
            {
                SetColorOnTerminal(ColorOutput.R, data, ColorOutput.B, ColorOutput.A);
            }
        }

        [InputTerminal(Direction.North)]
        public void BlueInputTerminal_DataChanged(float data)
        {
            if (ColorOutput != null)
            {
                SetColorOnTerminal(ColorOutput.R, ColorOutput.G, data, ColorOutput.A);
            }
        }

        [InputTerminal(Direction.West)]
        public void AlphaInputTerminal_DataChanged(float data)
        {
            if (ColorOutput != null)
            {
                SetColorOnTerminal(ColorOutput.R, ColorOutput.G, ColorOutput.B, data);
            }
        }

        protected override void MouseEnteredNode()
        {
            IsColorPickerVisible = true;
        }

        protected override void MouseLeftNode()
        {
            IsColorPickerVisible = false;
        }

        private void SetColorOnTerminal(float floatR, float floatG, float floatB, float floatA)
        {
            ColorOutput = new Color(floatR, floatG, floatB, floatA);
            var backgroundR = (byte)(floatR * 255.0f);
            var backgroundG = (byte)(floatG * 255.0f);
            var backgroundB = (byte)(floatB * 255.0f);
            var backgroundA = (byte)(floatA * 255.0f);
            var backgroundColor = System.Windows.Media.Color.FromArgb(backgroundA, backgroundR, backgroundG, backgroundB);
            View?.Dispatcher.Invoke(() =>
            {
                SelectedColorBrush.Color = backgroundColor;
            });
        }
    }
}