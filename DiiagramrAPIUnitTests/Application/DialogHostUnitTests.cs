using DiiagramrAPI.Application;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class DialogHostUnitTests : UnitTest
    {
        [Fact]
        public void OpenDialog_DialogActive()
        {
            var fakeDialog = new FakeDialog();
            var dialogHost = new DialogHost();
            dialogHost.OpenDialog(fakeDialog);

            Assert.Equal(fakeDialog, dialogHost.ActiveDialog);
        }

        [Fact]
        public void OpenDialog_ActiveDialogHostSetToThis()
        {
            var fakeDialog = new FakeDialog();
            var dialogHost = new DialogHost();
            dialogHost.OpenDialog(fakeDialog);

            Assert.Equal(dialogHost, fakeDialog.CurrentDialogHost);
        }

        [Fact]
        public void DialogOpen_CloseDialog_NoDialogActive()
        {
            var fakeDialog = new FakeDialog();
            var dialogHost = new DialogHost();
            dialogHost.OpenDialog(fakeDialog);
            Assert.Equal(fakeDialog, dialogHost.ActiveDialog);

            dialogHost.CloseDialog();

            Assert.Null(dialogHost.ActiveDialog);
        }

        [Fact]
        public void TwoDialogsOpen_CloseDialog_FirstOpenDialogActive()
        {
            var fakeDialog1 = new FakeDialog();
            var fakeDialog2 = new FakeDialog();
            var dialogHost = new DialogHost();
            dialogHost.OpenDialog(fakeDialog1);
            dialogHost.OpenDialog(fakeDialog2);
            Assert.Equal(fakeDialog2, dialogHost.ActiveDialog);

            dialogHost.CloseDialog();

            Assert.Equal(fakeDialog1, dialogHost.ActiveDialog);
        }

        [Fact]
        public void TwoDialogsOpen_CloseDialogTwice_NoActiveDialog()
        {
            var fakeDialog1 = new FakeDialog();
            var fakeDialog2 = new FakeDialog();
            var dialogHost = new DialogHost();
            dialogHost.OpenDialog(fakeDialog1);
            dialogHost.OpenDialog(fakeDialog2);
            Assert.Equal(fakeDialog2, dialogHost.ActiveDialog);

            dialogHost.CloseDialog();
            dialogHost.CloseDialog();

            Assert.Null(dialogHost.ActiveDialog);
        }
    }
}