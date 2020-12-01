using VisualDrop.AudioProcessing;

namespace VisualDrop
{
    public enum WindowFunctionType
    {
        None,
        Hanning,
        Hamming,
        HanningPeriodic,
        HammingPeriodic,
    }

    public static class WindowFunctionTypeExtensions
    {
        public static WindowFunction GetWindowFunctionFromType(this WindowFunctionType type) => type switch
        {
            WindowFunctionType.None => WindowFunctions.None,
            WindowFunctionType.Hanning => WindowFunctions.Hanning,
            WindowFunctionType.Hamming => WindowFunctions.Hamming,
            WindowFunctionType.HanningPeriodic => WindowFunctions.HanningPeriodic,
            WindowFunctionType.HammingPeriodic => WindowFunctions.HammingPeriodic,
            _ => WindowFunctions.None,
        };
    }
}