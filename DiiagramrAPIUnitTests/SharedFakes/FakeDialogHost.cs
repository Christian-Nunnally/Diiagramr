using DiiagramrAPI.Application;

namespace DiiagramrAPIUnitTests
{
    internal class FakeDialogHost : DialogHostBase
    {
        public override Dialog ActiveDialog { get; set; }

        public override void CloseDialog()
        {
        }

        public override void OpenDialog(Dialog dialog)
        {
        }
    }
}