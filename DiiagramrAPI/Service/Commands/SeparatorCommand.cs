using DiiagramrAPI.ViewModel;

namespace DiiagramrAPI.Service.Commands
{
    public class FileSeparatorCommand1 : SeparatorCommand
    {
        public override string Parent => "File";
        public override float Weight => 0.6f;
    }

    public class FileSeparatorCommand2 : SeparatorCommand
    {
        public override string Parent => "File";
        public override float Weight => 0.3f;
    }

    public abstract class SeparatorCommand : DiiagramrCommand
    {
        public override string Name => "";

        public override void Execute(ShellViewModel shell)
        {
        }
    }
}
