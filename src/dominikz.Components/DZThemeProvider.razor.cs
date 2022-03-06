using dominikz.Components.Models;
using Microsoft.AspNetCore.Components;


namespace dominikz.Components
{
    public partial class DZThemeProvider
    {
        [Parameter]
        public DZTheme Theme { get; set; }

        public DZThemeProvider()
        {

        }
    }
}
