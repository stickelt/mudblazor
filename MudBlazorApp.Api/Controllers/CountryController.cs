using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazorApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CountryController> _logger;

        public CountryController(IConfiguration configuration, ILogger<CountryController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryModel>>> GetCountries()
        {
            var countries = new List<CountryModel>();
            
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest("Database connection string is not configured.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("SELECT Country_ID, Name, Verfied_Mode_ID FROM Country", connection);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        countries.Add(new CountryModel
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Verified = reader.GetInt16(2) == 1
                        });
                    }
                }

                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching countries from database");
                return StatusCode(500, "Error accessing the database. See server logs for details.");
            }
        }
    }

    public class CountryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Verified { get; set; }
    }
}
