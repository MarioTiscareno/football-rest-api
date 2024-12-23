using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams.Queries;

public record GetTeamPlayersQuery(string Id) : IRequest<IEnumerable<TeamPlayerResponse>> { }

public class GetTeamPlayersQueryHandler
    : IRequestHandler<GetTeamPlayersQuery, IEnumerable<TeamPlayerResponse>>
{
    private readonly ITeamDb db;

    public GetTeamPlayersQueryHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<IEnumerable<TeamPlayerResponse>>> HandleAsync(
        GetTeamPlayersQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Get(request.Id)
            .Map(
                t =>
                    t.Players.Select(
                        p =>
                            new TeamPlayerResponse(p.Id, p.Name, p.HeightInCm, p.Age, p.Nationality)
                    )
            );

        return Task.FromResult(result);
    }
}

public class GetTeamPlayersQueryValidator : AbstractValidator<GetTeamPlayersQuery>
{
    public GetTeamPlayersQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
