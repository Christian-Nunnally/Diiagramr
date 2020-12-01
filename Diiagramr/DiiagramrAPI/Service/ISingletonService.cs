namespace DiiagramrAPI.Service
{
    /// <summary>
    /// Service interface that allows implementations to automatically get picked up by the composition container.
    /// This enables testing because you can easily swap out implementations of the interfaces in the composition container for tests.
    /// </summary>
    /// <remarks>
    /// Should be implemented by a service interface, e.g. <code>interface IService : IDiiagramrService</code>
    /// then implementations of "ISingletonService" will be automatically found and added to the composition catalog.
    /// </remarks>
    public interface ISingletonService
    {
    }
}