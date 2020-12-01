namespace DiiagramrAPI.Application.Dialogs
{
    /// <summary>
    /// An empty dialog that only shows the title. Useful for popup notifications.
    /// </summary>
    public class NotificationDialog : Dialog
    {
        /// <summary>
        /// Creates a new instance of <see cref="NotificationDialog"/>.
        /// </summary>
        /// <param name="notification">The notification string.</param>
        public NotificationDialog(string notification)
        {
            Title = notification;
        }

        /// <inheritdoc/>
        public override int MaxHeight => 0;

        /// <inheritdoc/>
        public override int MaxWidth => 100;

        /// <inheritdoc/>
        public override string Title { get; set; }
    }
}