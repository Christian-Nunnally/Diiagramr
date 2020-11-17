namespace DiiagramrAPI.Service
{
    /// <summary>
    /// A service that allows implementations to provide a <see cref="ServiceBindingKey"/>
    /// that is be used to uniqueify themselves in the composition container.
    /// </summary>
    public interface IKeyedService
    {
        /// <summary>
        /// The key for this service instance.
        /// </summary>
        string ServiceBindingKey { get; }
    }
}