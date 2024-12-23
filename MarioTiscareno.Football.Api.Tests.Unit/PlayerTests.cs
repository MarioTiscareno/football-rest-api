using FluentAssertions;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Players.Commands;
using MarioTiscareno.Football.Api.Players.Queries;

namespace MarioTiscareno.Football.Api.Tests.Unit;

public class PlayerTests
{
    [Fact]
    public async Task Insert_Player_Then_Get_Returns_Player()
    {
        // Arrange
        var db = new InMemoryPlayerDb();
        var insertHandler = new CreatePlayerCommandHandler(db);
        var getHandler = new GetPlayerQueryHandler(db);
        var insertCommand = new CreatePlayerCommand("Cristiano Ronaldo", 187, 39, "Portugal");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(p => new GetPlayerQuery(p.Id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<PlayerResponse>();
                p.Name.Should().Be("Cristiano Ronaldo");
                p.Age.Should().Be(39);
                p.Nationality.Should().Be("Portugal");
                p.HeightInCm.Should().Be(187);
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task Insert_Player_Then_Delete_Returns_Unit()
    {
        // Arrange
        var db = new InMemoryPlayerDb();
        var insertHandler = new CreatePlayerCommandHandler(db);
        var deleteHandler = new DeletePlayerCommandHandler(db);
        var insertCommand = new CreatePlayerCommand("Cristiano Ronaldo", 187, 39, "Portugal");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(p => new DeletePlayerCommand(p.Id))
            .BindAsync(q => deleteHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: unit =>
            {
                unit.Should().NotBeNull().And.BeOfType<Core.Unit>();
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task Update_Player_Then_Get_Returns_Updated_Player()
    {
        // Arrange
        var db = new InMemoryPlayerDb();
        var insertHandler = new CreatePlayerCommandHandler(db);
        var getHandler = new GetPlayerQueryHandler(db);
        var updateHandler = new UpdatePlayerCommandHandler(db);
        var insertCommand = new CreatePlayerCommand("Cristiano Ronaldo", 187, 39, "Portugal");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);
        var id = insertResult.Match(
            success: p => p.Id,
            failure: error => throw new InvalidOperationException() // something went wrong here, definitely a bug if this happens
        );

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(
                p =>
                    new UpdatePlayerCommand("Cristiano Ronaldo", 187, 40, "Portugal")
                    {
                        Id = p.Id
                    }
            )
            .BindAsync(q => updateHandler.HandleAsync(q))
            .MapAsync(p => new GetPlayerQuery(id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<PlayerResponse>();
                p.Name.Should().Be("Cristiano Ronaldo");
                p.Age.Should().Be(40);
                p.Nationality.Should().Be("Portugal");
                p.HeightInCm.Should().Be(187);
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task Patch_Player_Then_Get_Returns_Updated_Player()
    {
        // Arrange
        var db = new InMemoryPlayerDb();
        var insertHandler = new CreatePlayerCommandHandler(db);
        var getHandler = new GetPlayerQueryHandler(db);
        var updateHandler = new PatchPlayerCommandHandler(db);
        var insertCommand = new CreatePlayerCommand("Cristiano Ronaldo", 187, 39, "Portugal");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);
        var id = insertResult.Match(
            success: p => p.Id,
            failure: error => throw new InvalidOperationException() // something went wrong here, definitely a bug if this happens
        );

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(
                p =>
                    new PatchPlayerCommand(
                        Name: null,
                        HeightInCm: null,
                        Age: 40,
                        Nationality: null
                    )
                    {
                        Id = p.Id
                    }
            )
            .BindAsync(q => updateHandler.HandleAsync(q))
            .MapAsync(p => new GetPlayerQuery(id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<PlayerResponse>();
                p.Name.Should().Be("Cristiano Ronaldo");
                p.Age.Should().Be(40);
                p.Nationality.Should().Be("Portugal");
                p.HeightInCm.Should().Be(187);
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task GetAllPlayers_Returns_All_Player()
    {
        // Arrange
        var db = new InMemoryPlayerDb();
        var insertHandler = new CreatePlayerCommandHandler(db);
        var getAllHandler = new GetAllPlayersQueryHandler(db);
        var insertRonaldo = new CreatePlayerCommand("Cristiano Ronaldo", 187, 39, "Portugal");
        var insertMessi = new CreatePlayerCommand("Lionel Messi", 170, 37, "Argentina");
        var insertMbappe = new CreatePlayerCommand("Kylian Mbappe", 178, 26, "France");

        // Act
        await insertHandler.HandleAsync(insertRonaldo);
        await insertHandler.HandleAsync(insertMessi);
        await insertHandler.HandleAsync(insertMbappe);

        var result = await getAllHandler.HandleAsync(new GetAllPlayersQuery());

        // Assert
        result.Match(
            success: players =>
            {
                players
                    .Should()
                    .NotBeNull()
                    .And.BeAssignableTo<IEnumerable<PlayerResponse>>()
                    .And.HaveCount(3);
            },
            failure: error => Assert.Fail(error.Message)
        );
    }
}
