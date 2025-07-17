using Microsoft.AspNetCore.Components;
using MudBlazorApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MudBlazorApp.Pages
{
    public class CountryDetailBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        protected CountryModel? Country { get; set; }
        protected bool IsLoading { get; set; } = true;
        protected string ErrorMessage { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var apiUrl = $"http://localhost:5200/api/country/{Id}";
                Country = await Http.GetFromJsonAsync<CountryModel>(apiUrl);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading country details: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void BackToList()
        {
            Navigation.NavigateTo("/country");
        }
    }
}
