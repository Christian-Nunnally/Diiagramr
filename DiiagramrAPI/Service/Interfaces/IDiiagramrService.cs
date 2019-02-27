namespace DiiagramrAPI.Service.Interfaces
{
    /// <summary>
    /// Service interface that allows implementations to automatically get picked up by the composition container.
    /// </summary>
    /// <remarks>
    /// Should be implemented by a service interface, e.g. <code>interface IAmAService : IDiiagramrService</code>
    /// then implementations of "IAmAService" will be automatically found and added to the composition catalog.
    /// </remarks>
    public interface IService
    {
    }
}
