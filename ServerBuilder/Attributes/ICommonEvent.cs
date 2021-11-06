namespace ServerBuilder.Attributes
{
    public interface ICommonEvent<T>
    {
        T Type { get; }
        int[] RequiredPorts { get; }
    }
}
