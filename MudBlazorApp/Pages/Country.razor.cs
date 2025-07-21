using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazorApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;

namespace MudBlazorApp.Pages
{
    // Local definition as a workaround for the reference issue
    public class CountryRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool Verified { get; set; }
    }

    public partial class Country : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        [Parameter]
        public int? Id { get; set; }

        private List<CountryModel> _countries = new();
        private CountryModel? _selectedCountry;
        private bool _showingDetails;
        private bool _dialogVisible;
        private string _newCountryName = string.Empty;
        private bool _newCountryVerified = false;
        private bool _isEdit = false;
        private int _editingCountryId = 0;
        private string _sortOrder = "Name ASC";

        private IEnumerable<CountryModel> _sortedCountries
        {
            get
            {
                return _sortOrder switch
                {
                    "Name ASC" => _countries.OrderBy(c => c.Name),
                    "Name DESC" => _countries.OrderByDescending(c => c.Name),
                    "Id ASC" => _countries.OrderBy(c => c.Id),
                    "Id DESC" => _countries.OrderByDescending(c => c.Id),
                    "Custom" => _countries.OrderByDescending(c => c.Verified).ThenBy(c => c.Name),
                    _ => _countries
                };
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Id.HasValue && Id != _selectedCountry?.Id)
            {
                await LoadCountryDetails(Id.Value);
            }
            else if (!Id.HasValue)
            {
                _showingDetails = false;
                _selectedCountry = null;
            }
        }

        private async Task LoadCountriesAsync()
        {
            try
            {
                var result = await Http.GetFromJsonAsync<List<CountryModel>>("api/country");
                if (result != null)
                {
                    _countries = result;
                }
            }
            catch (System.Exception ex)
            {
                Snackbar.Add($"Error loading countries: {ex.Message}", Severity.Error);
            }
        }

        private async Task LoadCountryDetails(int countryId)
        {
            try
            {
                var result = await Http.GetFromJsonAsync<CountryModel>($"api/country/{countryId}");
                if (result != null)
                {
                    _selectedCountry = result;
                    _showingDetails = true;
                }
            }
            catch (System.Exception ex)
            {
                Snackbar.Add($"Error loading country details: {ex.Message}", Severity.Error);
            }
        }

        private void RowClicked(TableRowClickEventArgs<CountryModel> args)
        {
            if (args.Item != null)
            {
                NavigationManager.NavigateTo($"/country/{args.Item.Id}");
            }
        }

        private void AddNew()
        {
            _newCountryName = string.Empty;
            _newCountryVerified = false;
            _isEdit = false;
            _dialogVisible = true;
        }

        private void EditCountry(CountryModel country)
        {
            _newCountryName = country.Name;
            _newCountryVerified = country.Verified;
            _isEdit = true;
            _editingCountryId = country.Id;
            _dialogVisible = true;
        }

        private void CancelAddCountry()
        {
            _dialogVisible = false;
        }

        private async Task SaveCountryAsync()
        {
            if (string.IsNullOrWhiteSpace(_newCountryName))
            {
                Snackbar.Add("Country name cannot be empty.", Severity.Warning);
                return;
            }

            var countryRequest = new CountryRequest { Name = _newCountryName, Verified = _newCountryVerified };

            HttpResponseMessage response;
            if (_isEdit)
            {
                response = await Http.PutAsJsonAsync($"api/country/{_editingCountryId}", countryRequest);
            }
            else
            {
                response = await Http.PostAsJsonAsync("api/country", countryRequest);
            }

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add(_isEdit ? "Country updated successfully!" : "Country added successfully!", Severity.Success);
                _dialogVisible = false;
                await LoadCountriesAsync();
                StateHasChanged();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Snackbar.Add($"Error {(_isEdit ? "updating" : "adding")} country: {errorContent}", Severity.Error);
            }
        }

        private void BackToList()
        {
            NavigationManager.NavigateTo("/country");
        }
    }
}
