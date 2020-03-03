using DiiagramrAPI.Application;

namespace DiiagramrAPIUnitTests
{
    public class FakeDialog : Dialog
    {
        public override int MaxHeight => 0;

        public override int MaxWidth => 0;

        public override string Title { get; set; } = "Fake Dialog";
    }
}