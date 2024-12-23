namespace MarioTiscareno.Football.Api.Players;

public record PlayerResponse(string Id, string Name, int HeightInCm, int Age, string Nationality)
{
    public PlayerTeam? Team { get; init; }

    public record PlayerTeam(string Id, string Name);
}

public static class PlayerExtensions
{
    public static PlayerResponse ToResponse(this Player player) =>
        new(player.Id, player.Name, player.HeightInCm, player.Age, player.Nationality)
        {
            Team = player.Team is null
                ? null
                : new PlayerResponse.PlayerTeam(player.Team.Id, player.Team.Name)
        };
}
