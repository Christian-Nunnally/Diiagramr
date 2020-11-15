namespace VisualDrop.AudioProcessing
{
    public interface ISpectrumResultObserver
    {
        void ObserveSpectrumResults(float[] fftResults);
    }
}