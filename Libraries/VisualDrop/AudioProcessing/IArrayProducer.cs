namespace VisualDrop
{
    internal interface IArrayProducer
    {
        IArrayConsumer Consumer { get; set; }
    }
}