using FluentAssertions;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Teams;
using MarioTiscareno.Football.Api.Teams.Commands;
using MarioTiscareno.Football.Api.Teams.Queries;

namespace MarioTiscareno.Football.Api.Tests.Unit;

public class TeamTests
{
    [Fact]
    public async Task Insert_Team_Then_Get_Returns_Team()
    {
        // Arrange
        var db = new InMemoryTeamDb();
        var insertHandler = new CreateTeamCommandHandler(db);
        var getHandler = new GetTeamQueryHandler(db);
        var insertCommand = new CreateTeamCommand("Ajax", "Netherlands", "Eredivisie");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(p => new GetTeamQuery(p.Id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<TeamResponse>();
                p.Name.Should().Be("Ajax");
                p.Country.Should().Be("Netherlands");
                p.League.Should().Be("Eredivisie");
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task Insert_Team_Then_Delete_Returns_Unit()
    {
        // Arrange
        var db = new InMemoryTeamDb();
        var insertHandler = new CreateTeamCommandHandler(db);
        var deleteHandler = new DeleteTeamCommandHandler(db);
        var insertCommand = new CreateTeamCommand("Ajax", "Netherlands", "Eredivisie");

        // Act
        var insertResult = await insertHandler.HandleAsync(insertCommand);

        var result = await insertResult
            .ToResultAsync()
            .MapAsync(p => new DeleteTeamCommand(p.Id))
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
    public async Task Update_Team_Then_Get_Returns_Updated_Team()
    {
        // Arrange
        var db = new InMemoryTeamDb();
        var insertHandler = new CreateTeamCommandHandler(db);
        var getHandler = new GetTeamQueryHandler(db);
        var updateHandler = new UpdateTeamCommandHandler(db);
        var insertCommand = new CreateTeamCommand("Ajax", "Netherlands", "Eredivisie");

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
                    new UpdateTeamCommand("AFC Ajax", "Netherlands", "Eredivisie") { Id = p.Id }
            )
            .BindAsync(q => updateHandler.HandleAsync(q))
            .MapAsync(p => new GetTeamQuery(id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<TeamResponse>();
                p.Name.Should().Be("AFC Ajax");
                p.Country.Should().Be("Netherlands");
                p.League.Should().Be("Eredivisie");
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task Patch_Team_Then_Get_Returns_Updated_Team()
    {
        // Arrange
        var db = new InMemoryTeamDb();
        var insertHandler = new CreateTeamCommandHandler(db);
        var getHandler = new GetTeamQueryHandler(db);
        var updateHandler = new PatchTeamCommandHandler(db);
        var insertCommand = new CreateTeamCommand("Ajax", "Netherlands", "Eredivisie");

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
                    new PatchTeamCommand(Name: "AFC Ajax", Country: null, League: null)
                    {
                        Id = p.Id
                    }
            )
            .BindAsync(q => updateHandler.HandleAsync(q))
            .MapAsync(p => new GetTeamQuery(id))
            .BindAsync(q => getHandler.HandleAsync(q));

        // Assert
        result.Match(
            success: p =>
            {
                p.Should().NotBeNull().And.BeOfType<TeamResponse>();
                p.Name.Should().Be("AFC Ajax");
                p.Country.Should().Be("Netherlands");
                p.League.Should().Be("Eredivisie");
            },
            failure: error => Assert.Fail(error.Message)
        );
    }

    [Fact]
    public async Task GetAllTeams_Returns_All_Team()
    {
        // Arrange
        var db = new InMemoryTeamDb();
        var insertHandler = new CreateTeamCommandHandler(db);
        var getAllHandler = new GetAllTeamsQueryHandler(db);
        var insertAjax = new CreateTeamCommand("Ajax", "Netherlands", "Eredivisie");
        var insertPsv = new CreateTeamCommand("PSV", "Netherlands", "Eredivisie");

        // Act
        await insertHandler.HandleAsync(insertAjax);
        await insertHandler.HandleAsync(insertPsv);

        var result = await getAllHandler.HandleAsync(new GetAllTeamsQuery());

        // Assert
        result.Match(
            success: players =>
            {
                players
                    .Should()
                    .NotBeNull()
                    .And.BeAssignableTo<IEnumerable<TeamResponse>>()
                    .And.HaveCount(2);
            },
            failure: error => Assert.Fail(error.Message)
        );
    }
}
