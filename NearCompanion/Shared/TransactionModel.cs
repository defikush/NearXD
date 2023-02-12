using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearCompanion.Shared
{
    public class TransactionModel : SimpleTransactionModel
    {
        public string BlockHash { get; set; } = string.Empty;
        public double TGasSpent { get; set; } = 0;

        public List<ReceiptModel> Receipts { get; set; } = new List<ReceiptModel>();
    }

    public class ReceiptModel : SimpleReceiptModel 
    {
        public string BlockHash { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string TGasSpent { get; set; } = string.Empty;

        public List<ReceiptModel> ChildReceipts { get; set; } = new List<ReceiptModel>();
    }
}
