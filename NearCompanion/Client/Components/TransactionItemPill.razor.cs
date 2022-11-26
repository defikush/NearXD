using Microsoft.AspNetCore.Components;

namespace NearCompanion.Client.Components
{
    public partial class TransactionItemPill : ComponentBase
    {
        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public string? Color { get; set; }

        [Parameter]
        public string? Name { get; set; }

        [Parameter]
        public string? Text { get; set; }
    }
}
