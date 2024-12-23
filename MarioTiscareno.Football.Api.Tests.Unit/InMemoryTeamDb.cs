using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Tests.Unit;

/// <summary>
/// InMemory implementation of ITeamDb, not thread safe, only use for testing.
/// </summary>
public class InMemoryTeamDb : ITeamDb
{
    private List<Team> teams = new List<Team>();

    public ResultOf<Core.Unit> Delete(string id)
    {
        teams.RemoveAll(p => p.Id == id);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> DeleteAll()
    {
        teams.Clear();

        return Core.Unit.Value;
    }

    public ResultOf<Team> Get(string id)
    {
        var team = teams.Find(p => p.Id == id);

        return team is null ? new NotFoundInDbError("teams", id) : team;
    }

    public ResultOf<IEnumerable<Team>> Where(Func<Team, bool> predicate)
    {
        return new ResultOf<IEnumerable<Team>>(teams.Where(predicate));
    }

    public ResultOf<IEnumerable<Team>> GetAll()
    {
        return new ResultOf<IEnumerable<Team>>(teams);
    }

    public ResultOf<Core.Unit> Insert(Team team)
    {
        teams.Add(team);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> Update(Team team)
    {
        var existing = teams.Find(p => p.Id == team.Id);

        if (existing is null)
        {
            return new NotFoundInDbError("teams", team.Id);
        }

        teams.Remove(existing);
        teams.Add(team);

        return Core.Unit.Value;
    }

    public ResultOf<Core.Unit> Upsert(Team team)
    {
        var existing = teams.Find(p => p.Id == team.Id);

        if (existing is null)
        {
            return Insert(team);
        }

        return Update(team);
    }
}
