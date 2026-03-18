using FilmApi.Models;
using FilmApi.Services;
using FilmApi.Repositories;
using FilmApi.Tests.Builders;
using NSubstitute;
using Xunit;
using VerifyXunit;

namespace FilmApi.Tests;

/// <summary>
/// État initial du squelette : ce test vérifie un DTO complexe (Film avec Réalisateur, Acteurs, Genres)
/// via une longue série d'Assert.Equal.
/// </summary>
public class FilmDetailSnapshotTests
{
    [Fact]
    public async Task GetById_Returns_Complex_Film_Structure()
    {
        var substituteRepo = Substitute.For<IFilmRepository>();
        
        var director = new DirectorBuilder()
            .WithId("dir-1")
            .WithName("Denis", "Villeneuve")
            .WithNationality("CA")
            .Build(); 

        var actor1 = new ActorBuilder()
            .WithId("a1")
            .WithName("Timothée", "Chalamet")
            .WithRole("Paul Atréides")
            .Build();
            
        var actor2 = new ActorBuilder()
            .WithId("a2")
            .WithName("", "Zendaya")
            .WithRole("Chani")
            .Build();

        var genre1 = new GenreBuilder()
            .WithId("g1")
            .WithName("Science-Fiction")
            .Build();
            
        var genre2 = new GenreBuilder()
            .WithId("g2") 
            .WithName("Aventure")
            .Build();
            
        var country = new CountryBuilder()
            .WithCode("US")
            .WithName("États-Unis")
            .Build();

        var film = new FilmBuilder()
            .WithId("film-abc-123")
            .WithTitle("Dune")
            .WithSummary("Sur la planète Arrakis...")
            .WithYear(2021)
            .WithDuration(155)
            .WithReleaseDate(new DateTime(2021, 9, 15))
            .WithDirector(director)
            .WithActors(actor1, actor2)
            .WithGenres(genre1, genre2)
            .WithProductionCountry(country)
            .Build();
            
        substituteRepo.GetByIdAsync("film-abc-123").Returns(film);

        var service = new FilmService(substituteRepo);
        var result = await service.GetByIdAsync("film-abc-123");

        Assert.NotNull(result);
        Assert.Equal("film-abc-123", result!.Id);
        Assert.Equal("Dune", result.Title);
        Assert.Equal("Sur la planète Arrakis...", result.Summary);
        Assert.Equal(2021, result.Year);
        Assert.Equal(155, result.DurationMinutes);
        Assert.Equal(new DateTime(2021, 9, 15), result.ReleaseDate);
        Assert.NotNull(result.Director);
        Assert.Equal("dir-1", result.Director.Id);
        Assert.Equal("Villeneuve", result.Director.LastName);
        Assert.Equal("Denis", result.Director.FirstName);
        Assert.Equal("CA", result.Director.Nationality);
        Assert.Equal(2, result.Actors.Count);
        Assert.Equal("Chalamet", result.Actors[0].LastName);
        Assert.Equal("Paul Atréides", result.Actors[0].Role);
        Assert.Equal(2, result.Genres.Count);
        Assert.Equal("Science-Fiction", result.Genres[0].Name);
        Assert.NotNull(result.ProductionCountry);
        Assert.Equal("US", result.ProductionCountry.Code);
        Assert.Equal("États-Unis", result.ProductionCountry.Name);
    }
}
