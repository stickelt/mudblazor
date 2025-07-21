using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .WithOrigins("http://localhost:5150")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // Allows all origins
        .AllowCredentials());
});

var app = builder.Build();

// Enable CORS
app.UseCors("CorsPolicy");

app.MapGet("/", () => "Minimal API is running!");

// Country list endpoint
app.MapGet("/api/country", async (IConfiguration config) =>
{
    try
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        using var connection = new SqlConnection(connectionString);
        var countries = await connection.QueryAsync<CountryModel>("SELECT Country_ID as Id, Name, Verfied_Mode_ID as Verified FROM Country");
        Console.WriteLine($"Found {countries.Count()} countries");
        return Results.Ok(countries);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting countries: {ex.Message}");
        return Results.Problem(ex.Message);
    }
});

// Get country by id
app.MapGet("/api/country/{id}", async (int id, IConfiguration config) =>
{
    try
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        using var connection = new SqlConnection(connectionString);
        var country = await connection.QueryFirstOrDefaultAsync<CountryModel>("SELECT Country_ID as Id, Name, Verfied_Mode_ID as Verified FROM Country WHERE Country_ID = @Id", new { Id = id });

        if (country == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(country);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Add new country
app.MapPost("/api/country", async (CountryRequest request, IConfiguration config) =>
{
    try
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        using var connection = new SqlConnection(connectionString);

        var query = "INSERT INTO Country (Name, Verfied_Mode_ID, Sort_Key) VALUES (@Name, @VerifiedModeId, @SortKey); SELECT CAST(SCOPE_IDENTITY() as int);";
        var newId = await connection.ExecuteScalarAsync<int>(query, new { Name = request.Name, VerifiedModeId = request.Verified ? 1 : 0, SortKey = 0 });

        var newCountry = new CountryModel
        {
            Id = newId,
            Name = request.Name,
            Verified = request.Verified,
            Mode = "Standard",
            CreatedOn = DateTime.Now,
            LastModified = DateTime.Now
        };

        return Results.Created($"/api/country/{newId}", newCountry);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Update country by id
app.MapPut("/api/country/{id}", async (int id, CountryRequest request, IConfiguration config) =>
{
    try
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        using var connection = new SqlConnection(connectionString);

        var query = "UPDATE Country SET Name = @Name, Verfied_Mode_ID = @VerifiedModeId WHERE Country_ID = @Id";
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id, Name = request.Name, VerifiedModeId = request.Verified ? 1 : 0 });

        if (rowsAffected == 0)
        {
            return Results.NotFound();
        }

        var updatedCountry = new CountryModel
        {
            Id = id,
            Name = request.Name,
            Verified = request.Verified,
            Mode = "Standard",
            CreatedOn = DateTime.Now,
            LastModified = DateTime.Now
        };

        return Results.Ok(updatedCountry);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


Console.WriteLine("API server starting on http://localhost:5200");
app.Run("http://localhost:5200");

