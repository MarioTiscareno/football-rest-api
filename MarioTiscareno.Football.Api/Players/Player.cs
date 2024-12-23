using LiteDB;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Players;

public record Player(
    [BsonId] string Id,
    string Name,
    int HeightInCm,
    int Age,
    string Nationality
)
{
    [BsonRef("teams")]
    public Team? Team { get; init; }
}
