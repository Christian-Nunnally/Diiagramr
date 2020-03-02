namespace DiiagramrAPI.Application
{
    /// <summary>
    /// Utility class for installing temporary (or more long term) feature toggles.
    /// </summary>
    public static class FeatureToggles
    {
        public static bool WirePropagationAnimationFeatureOn { get; } = true;
    }
}