using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams.Commands;

public record CreateTeamCommand(string Name, string Country, string League)
    : IRequest<TeamResponse>;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamResponse>
{
    private readonly ITeamDb db;

    public CreateTeamCommandHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<TeamResponse>> HandleAsync(
        CreateTeamCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var team = new Team(
            Guid.NewGuid().ToString("N"),
            request.Name,
            request.Country,
            request.League
        );

        db.Insert(team);

        var response = team.ToResponse();

        return Task.FromResult(new ResultOf<TeamResponse>(response));
    }
}

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.League).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
