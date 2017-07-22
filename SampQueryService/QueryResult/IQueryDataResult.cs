namespace SampQueryService
{
    public interface IQueryDataResult
    {
        void Deserialize(byte[] data);
    }
}