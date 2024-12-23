using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Players.Queries;

public record GetPlayerQuery(string Id) : IRequest<PlayerResponse> { }

public class GetPlayerQueryHandler : IRequestHandler<GetPlayerQuery, PlayerResponse>
{
    private readonly IPlayerDb db;

    public GetPlayerQueryHandler(IPlayerDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<PlayerResponse>> HandleAsync(
        GetPlayerQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Get(request.Id).Map(p => p.ToResponse());

        return Task.FromResult(result);
    }
}

public class GetPlayerQueryValidator : AbstractValidator<GetPlayerQuery>
{
    public GetPlayerQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
