using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class GenreBuilder
{
    private string _id = "genre-1";
    private string _name = "Sci-Fi";

    public GenreBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public GenreBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Genre Build()
    {
        return new Genre
        {
            Id = _id,
            Name = _name
        };
    }
}

