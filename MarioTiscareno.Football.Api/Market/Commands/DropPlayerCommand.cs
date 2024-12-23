using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using MarioTiscareno.Football.Api.Players;
using MarioTiscareno.Football.Api.Teams;

namespace MarioTiscareno.Football.Api.Market.Commands;

/// <summary>
/// Command for dropping a player from a team
/// </summary>
/// <param name="TeamId"></param>
/// <param name="PlayerId"></param>
public record DropPlayerCommand(string TeamId, string PlayerId) : IRequest<Unit>;

/// <summary>
/// Drops a player from a team
/// </summary>
public class DropPlayerCommandHandler : IRequestHandler<DropPlayerCommand, Unit>
{
    private readonly IMarketDb marketDb;

    private readonly IPlayerDb playerDb;

    private readonly ITeamDb teamDb;

    public DropPlayerCommandHandler(IMarketDb marketDb, IPlayerDb playerDb, ITeamDb teamDb)
    {
        this.marketDb = marketDb;
        this.playerDb = playerDb;
        this.teamDb = teamDb;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        DropPlayerCommand request,
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
                            $"Failed to drop player {request.PlayerId} from team {request.TeamId} because the player was not found."
                        ),
                    _ => err
                };
            });

        var team = teamDb
            .Get(request.TeamId)
            // check if team exists
            .MapError(err =>
            {
                return err switch
                {
                    NotFoundInDbError
                        => new InvalidMarketOperationError(
                            $"Failed to drop player {request.PlayerId} from team {request.TeamId} because the team was not found."
                        ),
                    _ => err
                };
            })
            // check if player is signed to the team
            .Bind(t =>
            {
                if (!t.Players.Any(p => p.Id == request.PlayerId))
                {
                    return new InvalidMarketOperationError(
                        $"Failed to drop player {request.PlayerId} from team {request.TeamId} because the player is not signed to the team."
                    );
                }

                return new ResultOf<Team>(t);
            });

        var result = player
            .Bind(player => team.Map(team => (player, team)))
            .Bind(r => marketDb.DropPlayer(r.player, r.team));

        return Task.FromResult(result);
    }
}

public class DropPlayerCommandValidator : AbstractValidator<DropPlayerCommand>
{
    public DropPlayerCommandValidator()
    {
        RuleFor(x => x.TeamId).NotEmpty();
        RuleFor(x => x.PlayerId).NotEmpty();
    }
}
