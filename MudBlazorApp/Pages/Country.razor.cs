using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MudBlazorApp.Pages
{
    public partial class Country : CountryBase
    {
     
    }
    public class CountryBase : ComponentBase
    {
        protected string SortOption = "custom";

        protected List<CountryModel> Countries = new()
        {
            new CountryModel { Id = -2, Name = "Afghanistan", Verified = true },
            new CountryModel { Id = 2, Name = "Albania", Verified = true },
            new CountryModel { Id = 3, Name = "Algeria", Verified = true },
            new CountryModel { Id = 4, Name = "Andorra", Verified = false },
            new CountryModel { Id = 5, Name = "Angola", Verified = true },
            new CountryModel { Id = 6, Name = "Argentina", Verified = true },
            new CountryModel { Id = 7, Name = "Armenia", Verified = false },
            // Add more stubbed entries as needed
        };

        protected void AddNew()
        {
            // Stub: handle add new
        }

        protected void Edit(CountryModel country)
        {
            // Stub: handle edit
        }

        protected void ViewDetails(CountryModel country)
        {
            // Stub: handle view details
        }

        public class CountryModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool Verified { get; set; }
        }
    }
}
