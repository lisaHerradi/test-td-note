using FilmApi.Models;

namespace FilmApi.Tests.Builders;

public class ActorBuilder
{
    private string _id = "actor-1";
    private string _lastName = "DiCaprio";
    private string _firstName = "Leonardo";
    private string _role = "Cobb";

    public ActorBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ActorBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public ActorBuilder WithRole(string role)
    {
        _role = role;
        return this;
    }

    public Actor Build()
    {
        return new Actor
        {
            Id = _id,
            LastName = _lastName,
            FirstName = _firstName,
            Role = _role
        };
    }
}

