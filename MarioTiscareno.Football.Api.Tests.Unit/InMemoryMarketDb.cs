using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Market;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Tests.Unit;

/// <summary>
/// InMemory implementation of ITeamDb, not thread safe, only use for testing.
/// </summary>
public class InMemoryMarketDb : IMarketDb
{
    private readonly InMemoryPlayerDb playerDb;

    private readonly InMemoryTeamDb teamDb;

    public InMemoryMarketDb(InMemoryPlayerDb playerDb, InMemoryTeamDb teamDb)
    {
        this.playerDb = playerDb;
        this.teamDb = teamDb;
    }

    public ResultOf<Core.Unit> DropPlayer(Player player, Team team)
    {
        var updatedPlayer = player with { Team = null };
        playerDb.Update(updatedPlayer);

        team.Players.RemoveAll(p => p.Id == player.Id);
        teamDb.Update(team);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> SignPlayer(Player player, Team team)
    {
        teamDb
            .Where(teams => teams.Players.Contains(player))
            // remove player from origin team if exists
            .Bind(teams =>
            {
                var t = teams.SingleOrDefault();

                if (t is null)
                {
                    return Core.Unit.Value;
                }

                t.Players.Remove(player);
                return teamDb.Update(t);
            });

        team.Players.Add(player);
        var updateTeamResult = teamDb.Update(team);

        var updatedPlayer = player with { Team = team };
        var updatePlayerResult = playerDb.Update(updatedPlayer);

        return updateTeamResult.Bind(_ => updatePlayerResult);
    }
}
