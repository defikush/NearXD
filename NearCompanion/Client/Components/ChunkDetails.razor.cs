using Microsoft.AspNetCore.Components;
using NearCompanion.Shared;
using Radzen.Blazor;

namespace NearCompanion.Client.Components
{
    public partial class ChunkDetails : ComponentBase
    {
        [Parameter]
        public ChunkModel? Model { get; set; } = null;
    }
}