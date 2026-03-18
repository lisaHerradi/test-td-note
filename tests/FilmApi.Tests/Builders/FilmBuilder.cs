using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class FilmBuilder
{
    private string _id = "film-1";
    private string _title = "Inception";
    private string _summary = "Dreams within dreams";
    private int _year = 2010;
    private int _durationMinutes = 148;
    private DateTime? _releaseDate = new DateTime(2010, 7, 16);
    private Director _director = new DirectorBuilder().Build();
    private List<Genre> _genres = new() { new GenreBuilder().Build() };
    private List<Actor> _actors = new() { new ActorBuilder().Build() };
    private Country? _productionCountry = new CountryBuilder().Build();

    public FilmBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public FilmBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public FilmBuilder WithSummary(string summary)
    {
        _summary = summary;
        return this;
    }
    
    public FilmBuilder WithYear(int year)
    {
        _year = year;
        // Update ReleaseDate to match the year, so repository filters work correctly
        if (_releaseDate.HasValue)
        {
            try 
            {
                _releaseDate = new DateTime(year, _releaseDate.Value.Month, _releaseDate.Value.Day);
            }
            catch
            {
                // In case of leap year issues etc, safe fallback
                _releaseDate = new DateTime(year, 1, 1);
            }
        }
        else
        {
            _releaseDate = new DateTime(year, 1, 1);
        }
        return this;
    }

    public FilmBuilder WithDuration(int minutes)
    {
        _durationMinutes = minutes;
        return this;
    }

    public FilmBuilder WithReleaseDate(DateTime? date)
    {
        _releaseDate = date;
        return this;
    }

    public FilmBuilder WithDirector(Director director)
    {
        _director = director;
        return this;
    }

    public FilmBuilder WithGenres(params Genre[] genres)
    {
        _genres = genres.ToList();
        return this;
    }

     public FilmBuilder WithActors(params Actor[] actors)
    {
        _actors = actors.ToList();
        return this;
    }

    public FilmBuilder WithProductionCountry(Country country)
    {
        _productionCountry = country;
        return this;
    }

    public Film Build()
    {
        return new Film
        {
            Id = _id,
            Title = _title,
            Summary = _summary, 
            Year = _year,
            DurationMinutes = _durationMinutes,
            ReleaseDate = _releaseDate,
            Director = _director,
            Genres = _genres,
            Actors = _actors,
            ProductionCountry = _productionCountry
        };
    }
}
