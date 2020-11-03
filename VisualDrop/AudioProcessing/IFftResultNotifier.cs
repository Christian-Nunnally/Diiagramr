namespace VisualDrop
{
    internal interface IFftResultNotifier
    {
        void Subscribe(IFftResultObserver observer);

        void Unsubscribe(IFftResultObserver observer);
    }
}