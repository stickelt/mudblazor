using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Microsoft.JSInterop;
using MudBlazor;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using MudBlazorApp.Models;

namespace MudBlazorApp.Pages
{
    public partial class Country : CountryBase
    {
     
    }
    public class CountryBase : ComponentBase
    {
        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        protected string SortOption = "custom";
        protected List<CountryModel> Countries { get; set; } = new();
        protected bool IsLoading { get; set; } = true;
        protected string ErrorMessage { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        private async Task LoadCountriesAsync()
        {
            IsLoading = true;
            Countries.Clear();
            ErrorMessage = string.Empty;

            try
            {   
                // Fetch data from our API
                var apiUrl = "http://localhost:5200/api/country"; // Using our minimal API
                var result = await Http.GetFromJsonAsync<List<CountryModel>>(apiUrl);
                
                if (result != null)
                {
                    Countries = result;
                    Snackbar.Add($"Loaded {Countries.Count} countries from database", Severity.Success);
                }
                else
                {
                    ErrorMessage = "No countries returned from API";
                    Snackbar.Add(ErrorMessage, Severity.Warning);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading countries from API: {ex.Message}";
                Snackbar.Add(ErrorMessage, Severity.Error);
                
                // Fallback to mock data if API fails
                Countries = new List<CountryModel>()
                {
                    new CountryModel { Id = 1, Name = "Afghanistan (Mock)", Verified = true },
                    new CountryModel { Id = 2, Name = "Albania (Mock)", Verified = true },
                    new CountryModel { Id = 3, Name = "Algeria (Mock)", Verified = true },
                    new CountryModel { Id = 4, Name = "Andorra (Mock)", Verified = true },
                };
                
                Snackbar.Add("Loaded mock data as fallback", Severity.Info);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected void AddNew()
        {
            // Stub: handle add new
            Snackbar.Add("Add New functionality not implemented yet", Severity.Info);
        }

        protected void Edit(CountryModel country)
        {
            // Stub: handle edit
        }

        protected void ViewDetails(CountryModel country)
        {
            Navigation.NavigateTo($"/country/{country.Id}");
        }



        public class ConfigSettings
        {
            public ConnectionStringsConfig? ConnectionStrings { get; set; }
        }

        public class ConnectionStringsConfig
        {
            public string? DefaultConnection { get; set; }
        }
    }
}
