using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams.Queries;

public record GetTeamQuery(string Id) : IRequest<TeamResponse> { }

public class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamResponse>
{
    private readonly ITeamDb db;

    public GetTeamQueryHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<TeamResponse>> HandleAsync(
        GetTeamQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Get(request.Id).Map(t => t.ToResponse());

        return Task.FromResult(result);
    }
}

public class GetTeamQueryValidator : AbstractValidator<GetTeamQuery>
{
    public GetTeamQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
