using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Market;
using MarioTiscareno.Football.Api.Market.Commands;
using MarioTiscareno.Football.Api.Teams.Commands;
using MarioTiscareno.Football.Api.Teams.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MarioTiscareno.Football.Api.Teams;

public static class TeamEndpoints
{
    public static WebApplication MapTeamEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet("api")
            .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var groupBuilder = app.MapGroup("api/v{apiVersion:apiVersion}/teams")
            .WithApiVersionSet(apiVersionSet);

        groupBuilder.MapGet(
            "",
            async (
                [FromServices] RequestPipeline pipeline,
                GetAllTeamsQueryHandler handler,
                CancellationToken ct
            ) =>
            {
                var query = new GetAllTeamsQuery();
                var result = await pipeline.RunAsync(query, handler.HandleAsync, ct);

                return result.Match(
                    teams => Results.Ok(teams),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapGet(
            "{id}",
            async (
                [FromRoute] string id,
                [FromServices] RequestPipeline pipeline,
                GetTeamQueryHandler handler,
                CancellationToken ct
            ) =>
            {
                var query = new GetTeamQuery(id);
                var result = await pipeline.RunAsync(query, handler.HandleAsync, ct);

                return result.Match(
                    team => Results.Ok(team),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            NotFoundInDbError notFoundError
                                => Results.Problem(notFoundError.Message, statusCode: 404),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapPost(
            "",
            async (
                [FromServices] RequestPipeline pipeline,
                [FromBody] CreateTeamCommand command,
                CreateTeamCommandHandler handler
            ) =>
            {
                var result = await pipeline.RunAsync(command, handler.HandleAsync);

                return result.Match(
                    team => Results.Created($"/api/v1/teams/{team.Id}", team),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapPut(
            "{id}",
            async (
                [FromServices] RequestPipeline pipeline,
                [FromRoute] string id,
                UpdateTeamCommand command,
                UpdateTeamCommandHandler handler
            ) =>
            {
                var result = await pipeline.RunAsync(
                    command with
                    {
                        Id = id
                    },
                    handler.HandleAsync
                );

                return result.Match(
                    _ => Results.NoContent(),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            NotFoundInDbError notFoundError
                                => Results.Problem(notFoundError.Message, statusCode: 404),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapPatch(
            "{id}",
            async (
                [FromServices] RequestPipeline pipeline,
                [FromRoute] string id,
                PatchTeamCommand command,
                PatchTeamCommandHandler handler
            ) =>
            {
                var result = await pipeline.RunAsync(
                    command with
                    {
                        Id = id
                    },
                    handler.HandleAsync
                );

                return result.Match(
                    _ => Results.NoContent(),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            NotFoundInDbError notFoundError
                                => Results.Problem(notFoundError.Message, statusCode: 404),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapDelete(
            "{id}",
            async (
                [FromRoute] string id,
                [FromServices] RequestPipeline pipeline,
                DeleteTeamCommandHandler handler,
                CancellationToken ct
            ) =>
            {
                var command = new DeleteTeamCommand(id);
                var result = await pipeline.RunAsync(command, handler.HandleAsync, ct);

                return result.Match(
                    _ => Results.NoContent(),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            NotFoundInDbError notFoundError
                                => Results.Problem(notFoundError.Message, statusCode: 404),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapGet(
            "{id}/players",
            async (
                [FromRoute] string id,
                [FromServices] RequestPipeline pipeline,
                GetTeamPlayersQueryHandler handler,
                CancellationToken ct
            ) =>
            {
                var query = new GetTeamPlayersQuery(id);
                var result = await pipeline.RunAsync(query, handler.HandleAsync, ct);

                return result.Match(
                    players => Results.Ok(players),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            NotFoundInDbError notFoundError
                                => Results.Problem(notFoundError.Message, statusCode: 404),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapPost(
            "{id}/sign-player",
            async (
                [FromServices] RequestPipeline pipeline,
                [FromBody] SignPlayerCommand command,
                SignPlayerCommandHandler handler,
                CancellationToken ct
            ) =>
            {
                var result = await pipeline.RunAsync(command, handler.HandleAsync, ct);

                return result.Match(
                    _ => Results.NoContent(),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            InvalidMarketOperationError marketError
                                => Results.Problem(marketError.Message, statusCode: 409),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        groupBuilder.MapPost(
            "{id}/drop-player",
            async (
                [FromServices] RequestPipeline pipeline,
                [FromBody] DropPlayerCommand command,
                DropPlayerCommandHandler handler,
                CancellationToken ct
            ) =>
            {
                var result = await pipeline.RunAsync(command, handler.HandleAsync, ct);

                return result.Match(
                    _ => Results.NoContent(),
                    error =>
                    {
                        return error switch
                        {
                            ValidationError validationError
                                => validationError.CreateProblemResult(),
                            InvalidMarketOperationError marketError
                                => Results.Problem(marketError.Message, statusCode: 409),
                            Error e => Results.Problem(e.Message)
                        };
                    }
                );
            }
        );

        return app;
    }
}
