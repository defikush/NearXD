namespace NearCompanion.Shared
{
    public class BlockModel
    {
        public string Author { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public ulong Height { get; set; }
        public uint LengthMs { get; set; } = 1000;
        public ulong GasPrice { get; set; }
        public decimal UtilizationPercentage { get; set; }
        public ulong TimestampMs { get; set; }

        public List<ChunkModel> Chunks { get; set; } = new List<ChunkModel>();
    }

    public class ChunkModel
    {
        public uint ShardId { get; set; }
        public decimal UtilizationPercentage { get; set; }
    }
}
