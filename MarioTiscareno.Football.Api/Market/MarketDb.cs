using LiteDB;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Market;

public interface IMarketDb
{
    ResultOf<Unit> DropPlayer(Player player, Team team);

    ResultOf<Unit> SignPlayer(Player player, Team team);
}

/// <summary>
/// Provides transactional market db operations between players and teams
/// </summary>
public class MarketDb : IMarketDb
{
    private readonly LiteDatabase db;

    private readonly ILiteCollection<Player> players;

    private readonly ILiteCollection<Team> teams;

    public MarketDb(LiteDatabase db)
    {
        this.db = db;

        players = db.GetCollection<Player>("players");
        teams = db.GetCollection<Team>("teams");
    }

    /// <summary>
    /// Adds a player to a team. If the player is already on a team, it will be removed from the selling team.
    /// If the player does not have a team, it will only be added to the signing team.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="team"></param>
    /// <returns></returns>
    public ResultOf<Unit> SignPlayer(Player player, Team team)
    {
        try
        {
            db.BeginTrans();

            // find if player is already on a team
            var originTeam = teams
                .Query()
                .Where(t => t.Players.Select(p => p.Id).Contains(player.Id))
                .SingleOrDefault();

            // remove player from selling team
            if (originTeam is not null)
            {
                originTeam.Players.Remove(player);
                teams.Update(originTeam);
            }

            // add player to signing team
            team.Players.Add(player);

            // update player
            var updatedPlayer = player with
            {
                Team = team
            };

            teams.Update(team);
            players.Update(updatedPlayer);

            db.Commit();

            return Unit.Value;
        }
        catch (Exception)
        {
            db.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Drops a player from a team
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="teamId"></param>
    /// <returns></returns>
    public ResultOf<Unit> DropPlayer(string playerId, string teamId)
    {
        try
        {
            db.BeginTrans();

            var player = players.FindById(playerId);

            // if the player is not found, we return an error
            if (player is null)
            {
                var error = new InvalidMarketOperationError(
                    $"Failed to drop player {playerId} from team {teamId} because the player was not found."
                );
                return new ResultOf<Unit>(error);
            }

            var team = teams.FindById(teamId);

            // if the team is not found, we return an error
            if (team is null)
            {
                var error = new InvalidMarketOperationError(
                    $"Failed to drop player {playerId} from team {teamId} because the team was not found."
                );
                return new ResultOf<Unit>(error);
            }

            if (!team.Players.Any(p => p.Id == playerId))
            {
                var error = new InvalidMarketOperationError(
                    $"Failed to drop player {playerId} from team {teamId} because the player does not belong to the team."
                );
                return new ResultOf<Unit>(error);
            }

            team.Players.Remove(player);
            teams.Update(team);

            db.Commit();

            return Unit.CreateResult();
        }
        catch (Exception)
        {
            db.Rollback();
            throw;
        }
    }

    public ResultOf<Unit> DropPlayer(Player player, Team team)
    {
        try
        {
            db.BeginTrans();

            team.Players.Remove(player);
            teams.Update(team);

            var updatedPlayer = player with { Team = null };
            players.Update(updatedPlayer);

            db.Commit();

            return Unit.Value;
        }
        catch (Exception)
        {
            db.Rollback();
            throw;
        }
    }
}

public record InvalidMarketOperationError(string Message) : Error(Message);
