using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearCompanion.Shared
{
    public class ChunkModel
    {
        public ulong HeightCreated { get; set; }
        public ulong HeightIncluded { get; set; }
        public uint ShardId { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal UtilizationPercentage { get; set; }
        public Errors ChunkError { get; set; }

        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
        public List<ReceiptModel> Receipts { get; set; } = new List<ReceiptModel>();
    }

    public class TransactionModel
    {
        public string SignerId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;

        public List<ActionModel> Actions { get; set; } = new List<ActionModel>(); 
    }

    public class ActionModel
    {
        public ActionKind Kind { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public double Deposit { get; set; }
        public string AccessKeyPermission { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }

    public class ReceiptModel
    {
        public string PredecessorId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;

        public List<ActionModel> Actions { get; set; } = new List<ActionModel>();
    }

    public enum ActionKind
    {
        Undefined      = 0,
        FunctionCall   = 1,
        Transfer       = 2,
        DeployContract = 3,
        CreateAccount  = 4,
        DeleteAccount  = 5,
        AddKey         = 6,
        DeleteKey      = 7,
        Stake          = 8
    }
}
