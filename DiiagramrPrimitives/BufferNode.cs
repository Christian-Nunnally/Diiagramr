using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;

namespace DiiagramrPrimitives
{
    public class BufferNode : Node
    {
        public BufferNode() : base()
        {
            Width = 30;
            Height = 30;
            Name = "Buffer";
            ResizeEnabled = true;
            BufferLength = 2;
        }

        [NodeSetting]
        [OutputTerminal(Direction.South)]
        public object[] Buffer { get; set; } = new object[1];

        [NodeSetting]
        public int BufferLength { get; set; }

        [InputTerminal(Direction.North)]
        public void AddValue(object data)
        {
            if (Buffer.Length != BufferLength)
            {
                UpdateBufferLength();
            }

            var modifiedBuffer = Buffer;
            for (int i = modifiedBuffer.Length - 1; i > 1; i--)
            {
                modifiedBuffer[i] = modifiedBuffer[i - 1];
            }
            modifiedBuffer[0] = data;
            Buffer = null;
            Buffer = modifiedBuffer;

            OnPropertyChanged(nameof(Buffer));
        }

        [InputTerminal(Direction.East)]
        public void SetBufferLength(int bufferLength)
        {
            if (bufferLength > 0)
            {
                BufferLength = bufferLength;
            }
        }

        private void UpdateBufferLength()
        {
            var oldBuffer = Buffer;
            Buffer = new object[BufferLength];
            int i = 0;
            foreach (var item in oldBuffer)
            {
                if (i < BufferLength)
                {
                    Buffer[i++] = item;
                }
            }
        }
    }
}