using FluentAssertions;
using MarioTiscareno.Football.Api.Market.Commands;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Tests.Unit;

public class MarketTests
{
    [Fact]
    public async Task Sign_Player_Adds_Player_To_Team()
    {
        // Arrange
        var teamDb = new InMemoryTeamDb();
        var playerDb = new InMemoryPlayerDb();
        var maketDb = new InMemoryMarketDb(playerDb, teamDb);

        var ajax = new Team(Guid.NewGuid().ToString("N"), "Ajax", "Netherlands", "Eredivisie");
        teamDb.Insert(ajax);

        var ronaldo = new Player(
            Guid.NewGuid().ToString("N"),
            "Cristiano Ronaldo",
            187,
            39,
            "Portugal"
        );
        playerDb.Insert(ronaldo);

        var signPlayerCommand = new SignPlayerCommand(ajax.Id, ronaldo.Id);
        var signPlayerHandler = new SignPlayerCommandHandler(maketDb, playerDb, teamDb);

        // Act
        var result = await signPlayerHandler.HandleAsync(signPlayerCommand);
        var team = teamDb.Get(ajax.Id);
        var player = playerDb.Get(ronaldo.Id);

        // Assert
        result
            .Bind(r => team.Bind(t => player.Map(p => (Player: p, Team: t))))
            .Match(
                success: res =>
                {
                    res.Player.Team.Should().BeEquivalentTo(res.Team);
                    res.Team.Players.Should().Contain(p => p.Id == res.Player.Id);
                },
                failure: err => Assert.Fail(err.Message)
            );
    }

    [Fact]
    public async Task Drop_Player_Removes_Player_From_Team()
    {
        // Arrange
        var teamDb = new InMemoryTeamDb();
        var playerDb = new InMemoryPlayerDb();
        var maketDb = new InMemoryMarketDb(playerDb, teamDb);

        var ajax = new Team(Guid.NewGuid().ToString("N"), "Ajax", "Netherlands", "Eredivisie");
        teamDb.Insert(ajax);

        var ronaldo = new Player(
            Guid.NewGuid().ToString("N"),
            "Cristiano Ronaldo",
            187,
            39,
            "Portugal"
        );
        playerDb.Insert(ronaldo);

        var signPlayerCommand = new SignPlayerCommand(ajax.Id, ronaldo.Id);
        var signPlayerHandler = new SignPlayerCommandHandler(maketDb, playerDb, teamDb);

        var dropPlayerCommand = new DropPlayerCommand(ajax.Id, ronaldo.Id);
        var dropPlayerHandler = new DropPlayerCommandHandler(maketDb, playerDb, teamDb);

        // Act
        await signPlayerHandler.HandleAsync(signPlayerCommand);
        var result = await dropPlayerHandler.HandleAsync(dropPlayerCommand);
        var team = teamDb.Get(ajax.Id);
        var player = playerDb.Get(ronaldo.Id);

        // Assert
        result
            .Bind(r => team.Bind(t => player.Map(p => (Player: p, Team: t))))
            .Match(
                success: res => res.Team.Players.Should().NotContain(p => p.Id == res.Player.Id),
                failure: err => Assert.Fail(err.Message)
            );
    }
}
