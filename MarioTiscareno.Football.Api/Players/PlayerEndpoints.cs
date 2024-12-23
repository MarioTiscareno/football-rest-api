using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players.Commands;
using MarioTiscareno.Football.Api.Players.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MarioTiscareno.Football.Api.Players;

public static class PlayerEndpoints
{
    public static WebApplication MapPlayerEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet("api")
            .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var groupBuilder = app.MapGroup("api/v{apiVersion:apiVersion}/players")
            .WithApiVersionSet(apiVersionSet);

        groupBuilder.MapGet(
            "",
            async (
                [FromServices] RequestPipeline pipeline,
                GetAllPlayersQueryHandler handler,
                CancellationToken ct
            ) =>
            {
                var query = new GetAllPlayersQuery();
                var result = await pipeline.RunAsync(query, handler.HandleAsync, ct);

                return result.Match(
                    players => Results.Ok(players),
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
                GetPlayerQueryHandler handler,
                CancellationToken ct
            ) =>
            {
                var query = new GetPlayerQuery(id);
                var result = await pipeline.RunAsync(query, handler.HandleAsync, ct);

                return result.Match(
                    player => Results.Ok(player),
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
                [FromBody] CreatePlayerCommand command,
                CreatePlayerCommandHandler handler
            ) =>
            {
                var result = await pipeline.RunAsync(command, handler.HandleAsync);

                return result.Match(
                    player => Results.Created($"/players/{player.Id}", player),
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
                UpdatePlayerCommand command,
                UpdatePlayerCommandHandler handler
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
                PatchPlayerCommand command,
                PatchPlayerCommandHandler handler
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
                DeletePlayerCommandHandler handler,
                CancellationToken ct
            ) =>
            {
                var command = new DeletePlayerCommand(id);
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

        return app;
    }
}
