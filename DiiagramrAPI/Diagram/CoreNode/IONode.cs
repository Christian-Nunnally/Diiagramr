using DiiagramrAPI.PluginNodeApi;

namespace DiiagramrAPI.Diagram.CoreNode
{
    public abstract class IoNode : PluginNode
    {
        public IoNode()
        {
            Id = StaticId++;
        }

        [PluginNodeSetting]
        public int Id { get; set; }

        private static int StaticId { get; set; }
    }
}
