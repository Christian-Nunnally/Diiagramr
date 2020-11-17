namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// A node that is either an input or an output to a diagram.
    /// </summary>
    public abstract class IoNode : Node
    {
        /// <summary>
        /// Creates a new instance of <see cref="IoNode"/>.
        /// </summary>
        public IoNode()
        {
            Id = StaticId++;
        }

        /// <summary>
        /// The unique ID of this input/output terminal.
        /// </summary>
        // TODO: Remove this as it doesn't seem to be used any more.
        [NodeSetting]
        public int Id { get; set; }

        private static int StaticId { get; set; }
    }
}