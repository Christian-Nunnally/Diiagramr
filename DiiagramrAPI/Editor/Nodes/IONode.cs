namespace DiiagramrAPI.Editor.Nodes
{
    public abstract class IoNode : Node
    {
        public IoNode()
        {
            Id = StaticId++;
        }

        [NodeSetting]
        public int Id { get; set; }

        private static int StaticId { get; set; }
    }
}
