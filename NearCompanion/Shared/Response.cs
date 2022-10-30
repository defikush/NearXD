namespace NearCompanion.Shared
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public bool Success => Error == Errors.None;
        public string Message { get; set; } = string.Empty;
        public Errors Error { get; set; } = Errors.None;
    }

    public enum Errors
    {
        None = 0,

        // General Errors
        UnknownError = 1,
        DeserializationError = 2,
        NullResponse = 3,
        Timeout = 4,
        NotSyncedYet = 5,
        ParseError = 6,
        InternalError = 7,

        //Block Errors
        UnknownBlock = 100,
        UnknownChunk = 101,
        InvalidShardId = 102,

        //Internal Errors
        NoBlocks = 200,
        NotEnoughBlocks = 201,
        PrunedBlock = 202
    }
}
