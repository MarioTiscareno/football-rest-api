using LiteDB;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Players;

/// <summary>
/// Player data
/// </summary>
public interface IPlayerDb
{
    public ResultOf<Unit> Insert(Player player);

    public ResultOf<Player> Get(string id);

    public ResultOf<IEnumerable<Player>> GetAll();

    public ResultOf<Unit> Update(Player player);

    public ResultOf<Unit> Upsert(Player player);

    public ResultOf<Unit> Delete(string id);

    public ResultOf<Unit> DeleteAll();
}

/// <summary>
/// LiteDB implementation of IPlayerDb
/// </summary>
public sealed class PlayerDb : IPlayerDb
{
    private readonly ILiteCollection<Player> players;

    public PlayerDb(LiteDatabase db)
    {
        players = db.GetCollection<Player>("players");
        players.EnsureIndex(x => x.Id);
    }

    public ResultOf<Unit> Delete(string id)
    {
        return players.Delete(id) ? Unit.Value : new NotFoundInDbError("players", id);
    }

    public ResultOf<Player> Get(string id)
    {
        var player = players.FindById(id);

        return player is null ? new NotFoundInDbError("players", id) : player;
    }

    public ResultOf<Unit> Insert(Player player)
    {
        players.Insert(player);

        return Unit.Value;
    }

    public ResultOf<Unit> Update(Player player)
    {
        return players.Update(player) ? Unit.Value : new NotFoundInDbError("players", player.Id);
    }

    public ResultOf<Unit> Upsert(Player player)
    {
        players.Upsert(player);

        return Unit.Value;
    }

    public ResultOf<IEnumerable<Player>> GetAll()
    {
        var allPlayers = players.Include(x => x.Team).FindAll();

        return new ResultOf<IEnumerable<Player>>(allPlayers);
    }

    public ResultOf<Unit> DeleteAll()
    {
        players.DeleteAll();

        return Unit.Value;
    }
}
