using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazorApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MudBlazorApp.Pages
{
    public partial class Country : CountryBase
    {
     
    }
    public class CountryBase : ComponentBase
    {
        protected string searchString = "";
    private bool sortNameByLength;
    protected int elementsCount = 0;
    
    [Inject]
    private HttpClient Http { get; set; } = default!;

    
    
    protected CountryModel? selectedCountry = null;
    protected bool isLoadingDetails = false;
    protected bool showingDetails = false;
    
    [Parameter]
    public int? Id { get; set; }

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

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
        
        protected override async Task OnParametersSetAsync()
        {
            // This runs every time the parameters change (including when the component first loads)
            // If ID parameter is provided, load the country details
            if (Id.HasValue)
            {
                await LoadCountryDetails(Id.Value);
            }
            else
            {
                // If no ID is provided, show the list view
                showingDetails = false;
                selectedCountry = null;
            }
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
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                // Fallback to mock data
                Countries = new List<CountryModel>
                {
                    new CountryModel { Id = 1, Name = "United States (Mock)", Verified = true },
                    new CountryModel { Id = 2, Name = "Canada (Mock)", Verified = true },
                    new CountryModel { Id = 3, Name = "Mexico (Mock)", Verified = true },
                    new CountryModel { Id = 4, Name = "United Kingdom (Mock)", Verified = true }
                };

                Snackbar.Add("Using mock data because API is not available", Severity.Warning);
                
                // If ID parameter is provided, try to find in mock data
                if (Id.HasValue)
                {
                    var mockCountry = Countries.FirstOrDefault(c => c.Id == Id.Value);
                    if (mockCountry != null)
                    {
                        selectedCountry = mockCountry;
                        showingDetails = true;
                    }
                }
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

        protected void BackToList()
        {
            // Navigate back to the list URL
            Navigation.NavigateTo("/country");
        }

        protected async Task LoadCountryDetails(int id)
    {
        try
        {
            isLoadingDetails = true;
            
            // Fetch the detailed country data
            var apiUrl = $"http://localhost:5200/api/country/{id}";
            selectedCountry = await Http.GetFromJsonAsync<CountryModel>(apiUrl);
            
            // Show details view
            showingDetails = true;
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading details: {ex.Message}", Severity.Error);
        }
        finally
        {
            isLoadingDetails = false;
        }
    }
    
    protected void ViewDetails(CountryModel country)
    {
        // Navigate to the country details URL
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
