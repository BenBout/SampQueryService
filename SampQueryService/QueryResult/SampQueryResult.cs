namespace SampQueryService.QueryResult
{
    public abstract class SampQueryResult : IQueryDataResult
    {
        public char OpCode { get; internal set; }
        public bool IsCompleted { get; internal set; }

        public abstract void Deserialize(byte[] data);
    }
}
