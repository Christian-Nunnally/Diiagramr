namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Utility class for installing temporary (or more long term) feature toggles.
    /// </summary>
    public static class FeatureToggles
    {
        /// <summary>
        /// Turns on dots that visually move across wires when the data changes. Will degrade performance.
        /// </summary>
        public static bool WirePropagationAnimationFeatureOn { get; } = true;
    }
}