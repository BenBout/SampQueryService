namespace SampQueryService
{
    public interface IQueryDataResult
    {
        char GetOpCode();
        bool Deserialize(byte[] data);
    }
}