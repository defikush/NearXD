using Microsoft.AspNetCore.Components;
using NearCompanion.Client.Services.Interfaces;
using NearCompanion.Shared;
using Radzen.Blazor;
using Radzen;

namespace NearCompanion.Client.Components
{
    public partial class Block : ComponentBase
    {
        private string authorTrimmed = string.Empty;

        [Inject]
        private IChunkService? chunkService { get; set; }

        [Inject]
        private DialogService? dialogService { get; set; }

        [Parameter]
        public BlockModel? Model { get; set; } = null;

        private string GetAuthorTrimmed()
        {
            if (Model == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(authorTrimmed))
            {
                return authorTrimmed;
            }

            if (!string.IsNullOrEmpty(Model.Author))
            {
                var split = Model.Author.Split('.');

                if (split.Count() > 0)
                {
                    authorTrimmed = split[0];
                }
                else
                {
                    authorTrimmed = Model.Author;
                }
            }

            return authorTrimmed = string.IsNullOrEmpty(authorTrimmed) ? "unknown" : authorTrimmed;
        }

        private async Task HandleChunkClick(int id, ulong height)
        {
            try
            {
                if (dialogService == null || chunkService == null)
                {
                    return;
                }

                var chunk = await chunkService.GetChunk(id, height);

                var result = await dialogService.OpenAsync<ChunkDetails>(
                    $"Chunk {id}", 
                    new Dictionary<string, object>() {{ "Model", chunk }}, 
                    new DialogOptions()
                    {
                        Style = "border-radius: 25px; box-shadow: 3px 3px 0px 0px #5FB8FF, 0 12px 16px 0 #CAE6FF, 0 17px 50px 0 #CAE6FF; margin: 10px 50px 10px 10px;",
                        Width = "1200px",
                        Height = "700px"
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
