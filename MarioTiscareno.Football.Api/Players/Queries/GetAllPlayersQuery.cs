using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Players.Queries;

public record GetAllPlayersQuery() : IRequest<IEnumerable<PlayerResponse>>;

public class GetAllPlayersQueryHandler
    : IRequestHandler<GetAllPlayersQuery, IEnumerable<PlayerResponse>>
{
    private readonly IPlayerDb db;

    public GetAllPlayersQueryHandler(IPlayerDb db) => this.db = db;

    public Task<ResultOf<IEnumerable<PlayerResponse>>> HandleAsync(
        GetAllPlayersQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.GetAll().Map(players => players.Select(p => p.ToResponse()));

        return Task.FromResult(result);
    }
}

public class GetAllPlayersQueryValidator : AbstractValidator<GetAllPlayersQuery>
{
    public GetAllPlayersQueryValidator() { }
}
