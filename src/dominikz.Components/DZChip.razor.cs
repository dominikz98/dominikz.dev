using Microsoft.AspNetCore.Components;


namespace dominikz.Components
{
    public partial class DZChip
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public DZChip()
        {

        }
    }
}
