namespace DiiagramrAPI.Editor.Diagrams
{
    public abstract class IoNode : Node
    {
        public IoNode()
        {
            Id = StaticId++;
        }

        [NodeSettingAttribute]
        public int Id { get; set; }

        private static int StaticId { get; set; }
    }
}