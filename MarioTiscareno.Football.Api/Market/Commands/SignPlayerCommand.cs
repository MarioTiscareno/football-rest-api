using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;
using Microsoft.AspNetCore.Mvc;

namespace MarioTiscareno.Football.Api.Market.Commands;

/// <summary>
/// Command for signing a player to a team
/// </summary>
/// <param name="TeamId"></param>
/// <param name="PlayerId"></param>
public record SignPlayerCommand([FromRoute(Name = "id")] string TeamId, string PlayerId)
    : IRequest<Unit>;

/// <summary>
/// Signs a player to a team
/// </summary>
public class SignPlayerCommandHandler : IRequestHandler<SignPlayerCommand, Unit>
{
    private readonly IMarketDb marketDb;

    private readonly IPlayerDb playerDb;

    private readonly ITeamDb teamDb;

    public SignPlayerCommandHandler(IMarketDb db, IPlayerDb playerDb, ITeamDb teamDb)
    {
        marketDb = db;
        this.playerDb = playerDb;
        this.teamDb = teamDb;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        SignPlayerCommand request,
        CancellationToken cancellationToken = default
    )
    {
        // PERSONAL NOTE:
        // Some developers may be annoyed by this functional style, it's not for everyone.
        // I like it, but I'm OK with declarative style if the project requires it.

        var player = playerDb
            .Get(request.PlayerId)
            // check if player exists
            .MapError(err =>
            {
                return err switch
                {
                    NotFoundInDbError
                        => new InvalidMarketOperationError(
                            $"Failed to sign player {request.PlayerId} to team {request.TeamId} because the player was not found."
                        ),
                    _ => err
                };
            })
            // check if player is already signed to this team
            .Bind(
                p =>
                    p.Team?.Id == request.TeamId
                        ? new InvalidMarketOperationError(
                            $"Player {request.PlayerId} is already signed to team {request.TeamId}."
                        )
                        : new ResultOf<Player>(p)
            );

        var signingTeam = teamDb
            .Get(request.TeamId)
            // check if team exists
            .MapError(err =>
            {
                return err switch
                {
                    NotFoundInDbError
                        => new InvalidMarketOperationError(
                            $"Failed to sign player {request.PlayerId} to team {request.TeamId} because the team was not found."
                        ),
                    _ => err
                };
            });

        var result = player
            .Bind(player => signingTeam.Map(team => (player, team)))
            .Bind(r => marketDb.SignPlayer(r.player, r.team));

        return Task.FromResult(result);
    }
}

public class SignPlayerCommandValidator : AbstractValidator<SignPlayerCommand>
{
    public SignPlayerCommandValidator()
    {
        RuleFor(x => x.TeamId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
