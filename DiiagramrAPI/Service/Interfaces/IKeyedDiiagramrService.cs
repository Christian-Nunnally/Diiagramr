namespace DiiagramrAPI.Service.Interfaces
{
    /// <summary>
    /// A service that allows implementations to provide a <see cref="ServiceBindingKey"/>
    /// that is be used to uniqueify themselves in the composition container.
    /// </summary>
    public interface IKeyedDiiagramrService
    {
        string ServiceBindingKey { get; }
    }
}
