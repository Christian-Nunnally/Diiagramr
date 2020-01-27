namespace DiiagramrAPI.Application.Dialogs
{
    public class NotificationDialog : Dialog
    {
        public NotificationDialog(string notification)
        {
            Title = notification;
        }

        public override int MaxHeight => 0;

        public override int MaxWidth => 100;

        public override string Title { get; set; }
    }
}