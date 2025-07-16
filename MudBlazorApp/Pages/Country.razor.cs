using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MudBlazorApp.Pages
{
    public class CountryBase : ComponentBase
    {
        protected string SelectedCountry { get; set; } = string.Empty;

        protected List<string> Countries { get; set; } = new List<string>
        {
            "United States",
            "Canada",
            "United Kingdom",
            "Germany",
            "France",
            "Japan",
            "Australia",
            "Brazil",
            "India",
            "China"
        };

        protected void ResetSelection()
        {
            SelectedCountry = string.Empty;
        }
    }
}
