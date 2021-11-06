namespace ServerBuilder.Attributes
{
    public interface IClientEvent<T> : ICommonEvent<T>
    {

        bool SilentRequiredPorts { get; }
    }
}
