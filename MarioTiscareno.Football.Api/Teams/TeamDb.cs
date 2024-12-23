using LiteDB;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams;

/// <summary>
/// Team data
/// </summary>
public interface ITeamDb
{
    ResultOf<Unit> Insert(Team team);

    ResultOf<Unit> Update(Team team);

    ResultOf<Unit> Delete(string id);

    ResultOf<Team> Get(string id);

    ResultOf<Unit> Upsert(Team team);

    ResultOf<IEnumerable<Team>> GetAll();

    ResultOf<Unit> DeleteAll();
}

/// <summary>
/// LiteDB implementation of ITeamDb
/// </summary>
public class TeamDb : ITeamDb
{
    private readonly ILiteCollection<Team> teams;

    public TeamDb(LiteDatabase db)
    {
        teams = db.GetCollection<Team>("teams");
        teams.EnsureIndex(x => x.Id);
    }

    public ResultOf<Unit> Delete(string id)
    {
        return teams.Delete(id)
            ? Unit.Value
            : new ResultOf<Unit>(new NotFoundInDbError("teams", id));
    }

    public ResultOf<Team> Get(string id)
    {
        var team = teams.Include(t => t.Players).FindById(id);

        return team is null ? new ResultOf<Team>(new NotFoundInDbError("teams", id)) : team;
    }

    public ResultOf<Unit> Insert(Team team)
    {
        teams.Insert(team);

        return Unit.Value;
    }

    public ResultOf<Unit> Update(Team team)
    {
        return teams.Update(team)
            ? Unit.Value
            : new ResultOf<Unit>(new NotFoundInDbError("teams", team.Id));
    }

    public ResultOf<Unit> Upsert(Team team)
    {
        teams.Upsert(team);

        return Unit.Value;
    }

    public ResultOf<IEnumerable<Team>> GetAll()
    {
        var allTeams = teams.Include(t => t.Players).FindAll();

        return new ResultOf<IEnumerable<Team>>(allTeams);
    }

    public ResultOf<Unit> DeleteAll()
    {
        teams.DeleteAll();

        return Unit.Value;
    }
}
