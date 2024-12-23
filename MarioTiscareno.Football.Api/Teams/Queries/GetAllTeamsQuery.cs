using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams.Queries;

public record GetAllTeamsQuery() : IRequest<IEnumerable<TeamResponse>>;

public class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, IEnumerable<TeamResponse>>
{
    private readonly ITeamDb db;

    public GetAllTeamsQueryHandler(ITeamDb db) => this.db = db;

    public Task<ResultOf<IEnumerable<TeamResponse>>> HandleAsync(
        GetAllTeamsQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.GetAll().Map(t => t.Select(t => t.ToResponse()));

        return Task.FromResult(result);
    }
}

public class GetAllTeamsQueryValidator : AbstractValidator<GetAllTeamsQuery>
{
    public GetAllTeamsQueryValidator() { }
}
