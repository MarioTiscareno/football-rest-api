using LiteDB;
using MarioTiscareno.Football.Api.Players;

namespace MarioTiscareno.Football.Api.Teams;

public record Team([BsonId] string Id, string Name, string Country, string League)
{
    [BsonRef("players")]
    public List<Player> Players { get; init; } = [];
}
