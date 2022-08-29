using Microsoft.AspNetCore.Components;
using NearCompanion.Shared;

namespace NearCompanion.Client.Components
{
    public partial class Block : ComponentBase
    {
        private string authorTrimmed = string.Empty;

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
    }
}
