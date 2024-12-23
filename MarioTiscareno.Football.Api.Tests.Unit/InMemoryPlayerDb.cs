using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players;

namespace MarioTiscareno.Football.Api.Tests.Unit;

/// <summary>
/// InMemory implementation of IPlayerDb, not thread safe, only use for testing.
/// </summary>
public class InMemoryPlayerDb : IPlayerDb
{
    private List<Player> players = new List<Player>();

    public ResultOf<Core.Unit> Delete(string id)
    {
        players.RemoveAll(p => p.Id == id);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> DeleteAll()
    {
        players.Clear();

        return Core.Unit.Value;
    }

    public ResultOf<Player> Get(string id)
    {
        var player = players.Find(p => p.Id == id);

        return player is null
            ? new ResultOf<Player>(new NotFoundInDbError("players", id))
            : player;
    }

    public ResultOf<IEnumerable<Player>> Where(Func<Player, bool> predicate)
    {
        return new ResultOf<IEnumerable<Player>>(players.Where(predicate));
    }

    public ResultOf<IEnumerable<Player>> GetAll()
    {
        return new ResultOf<IEnumerable<Player>>(players);
    }

    public ResultOf<Core.Unit> Insert(Player player)
    {
        players.Add(player);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> Update(Player player)
    {
        var existing = players.Find(p => p.Id == player.Id);

        if (existing is null)
        {
            return new ResultOf<Core.Unit>(new NotFoundInDbError("players", player.Id));
        }

        players.Remove(existing);
        players.Add(player);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> Upsert(Player player)
    {
        var existing = players.Find(p => p.Id == player.Id);

        if (existing is null)
        {
            return Insert(player);
        }

        return Update(player);
    }
}
