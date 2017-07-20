namespace SampQueryService
{
    public interface IQueryDataResult
    {
        char GetOpCode();
        void Deserialize(byte[] data);
    }
}