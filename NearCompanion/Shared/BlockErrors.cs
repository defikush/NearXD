using System.ComponentModel;

namespace NearCompanion.Shared
{
    public enum BlockErrors
    {
        [Description("Block successfully retrieved")]
        None = 0,

        [Description("The request to get the block has timed out")]
        Timeout = 1,

        [Description("The requested block does not exist")]
        Unknown = 2,

        [Description("The RPC node is not synced")]
        Unsynced = 3,

        [Description("Request could not be parsed")]
        ParseError = 4,

        [Description("The RPC node had an internal error")]
        InternalError = 5
    }
}
