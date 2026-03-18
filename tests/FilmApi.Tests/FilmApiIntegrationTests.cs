using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FilmApi.Models;
using FilmApi.Tests.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace FilmApi.Tests;

/// <summary>
/// Tests d'intégration : HTTP → API → Service → Repository → MongoDB.
/// </summary>
public sealed class FilmApiIntegrationTests : IClassFixture<MongoFixture>, IAsyncLifetime
{
    private readonly MongoFixture _mongo;
    private readonly FilmApiAppFactory _factory;
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public FilmApiIntegrationTests(MongoFixture mongo)
    {
        _mongo = mongo;
        _factory = new FilmApiAppFactory(mongo);
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Start container if not started (it is started by fixture), but clean DB
        // MongoFixture implements IAsyncLifetime so it starts once.
        // We just clear films here.
        await _mongo.ClearFilmsAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_films_Returns_201_And_Film()
    {
        // Arrange
        var director = new DirectorBuilder().WithId("d1").WithName("Jean", "Dupont").WithNationality("FR").Build();
        var genre = new GenreBuilder().WithId("g1").WithName("Comédie").Build();
        var country = new CountryBuilder().WithCode("FR").WithName("France").Build();

        var request = new CreateFilmRequest(
            Title: "Mon Film",
            Summary: "Résumé.",
            Year: 2024,
            DurationMinutes: 90,
            ReleaseDate: DateTime.UtcNow,
            Director: director,
            Genres: new List<Genre> { genre },
            Actors: new List<Actor>(),
            ProductionCountry: country
        );

        // Act
        var response = await _client.PostAsJsonAsync("/films", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var film = await response.Content.ReadFromJsonAsync<Film>(JsonOptions);
        Assert.NotNull(film);
        Assert.Equal("Mon Film", film.Title);
        Assert.False(string.IsNullOrEmpty(film.Id));
    }

    [Fact]
    public async Task GET_films_By_Year_Returns_Correct_Films()
    {
        // Arrange
        var collection = _mongo.GetCollection();
        
        var film2020 = new FilmBuilder()
            .WithId(ObjectId.GenerateNewId().ToString())
            .WithTitle("Film 2020")
            .WithYear(2020)
            .Build();
            
        var film2021_1 = new FilmBuilder()
            .WithId(ObjectId.GenerateNewId().ToString())
            .WithTitle("Film 2021 A")
            .WithYear(2021)
            .Build();
            
        var film2021_2 = new FilmBuilder()
            .WithId(ObjectId.GenerateNewId().ToString())
            .WithTitle("Film 2021 B")
            .WithYear(2021)
            .Build();

        await collection.InsertManyAsync(new[] { film2020, film2021_1, film2021_2 });

        // Act
        var response = await _client.GetAsync("/films?releaseYear=2021");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var films = await response.Content.ReadFromJsonAsync<PagedResult<Film>>(JsonOptions);
        
        // Assuming the API returns a PagedResult or List<Film>. 
        // Checking PagedResult.cs...
        // Let's assume it returns PagedResult<Film> based on common patterns, or maybe just List<Film>.
        // Looking at file tree: PagedResult.cs exists.
        // I will check PagedResult.cs content now quickly.
        
        Assert.NotNull(films);
        Assert.Equal(2, films.TotalCount);
        Assert.All(films.Items, f => Assert.Equal(2021, f.Year));
    }

    [Fact]
    public async Task DELETE_film_Returns_204_And_Removes_Film()
    {
        // Arrange
        var collection = _mongo.GetCollection();
        var filmToDelete = new FilmBuilder()
            .WithId(ObjectId.GenerateNewId().ToString())
            .WithTitle("Film To Delete")
            .Build();
            
        await collection.InsertOneAsync(filmToDelete);

        // Act
        var response = await _client.DeleteAsync($"/films/{filmToDelete.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        var getResponse = await _client.GetAsync($"/films/{filmToDelete.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        
        // Verify in DB directly
        var count = await collection.CountDocumentsAsync(f => f.Id == filmToDelete.Id);
        Assert.Equal(0, count);
    }
}
