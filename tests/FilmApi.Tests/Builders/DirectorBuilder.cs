using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class DirectorBuilder
{
    private string _id = "director-1";
    private string _lastName = "Nolan";
    private string _firstName = "Christopher";
    private string _nationality = "UK";
    private DateTime? _birthDate = new DateTime(1970, 7, 30);

    public DirectorBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public DirectorBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }
    
    public DirectorBuilder WithNationality(string nationality)
    {
        _nationality = nationality;   
        return this;
    }

    public Director Build()
    {
        return new Director
        {
            Id = _id,
            LastName = _lastName,
            FirstName = _firstName,
            Nationality = _nationality,
            BirthDate = _birthDate
        };
    }
}

