using Microsoft.AspNetCore.Components;
using NearCompanion.Shared;

namespace NearCompanion.Client.Components
{
    public partial class TransactionItem : ComponentBase
    {
        [Parameter]
        public TransactionModel? Model { get; set; }
    }
}
