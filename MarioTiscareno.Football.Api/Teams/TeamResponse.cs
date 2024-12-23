namespace MarioTiscareno.Football.Api.Teams;

public record TeamResponse(string Id, string Name, string Country, string League) { }

public static class TeamExtensions
{
    public static TeamResponse ToResponse(this Team team) =>
        new(team.Id, team.Name, team.Country, team.League);
}
