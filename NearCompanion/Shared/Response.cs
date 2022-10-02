namespace NearCompanion.Shared
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public bool Success => Error == Errors.None;
        public string Message { get; set; } = string.Empty;
        public Errors Error = Errors.None;
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

        //Internal Errors
        NoBlocks = 200,
        NotEnoughBlocks = 201
    }
}
