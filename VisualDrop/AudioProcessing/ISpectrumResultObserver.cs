using System.Collections.Generic;

namespace VisualDrop.AudioProcessing
{
    public interface ISpectrumResultObserver
    {
        void ObserveSpectrumResults(List<float> fftResults);
    }
}