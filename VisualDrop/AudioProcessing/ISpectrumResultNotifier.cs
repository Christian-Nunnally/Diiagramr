using VisualDrop.AudioProcessing;

namespace VisualDrop
{
    internal interface ISpectrumResultNotifier
    {
        void Subscribe(ISpectrumResultObserver subscriber);

        void Unsubscribe(ISpectrumResultObserver subscriber);
    }
}