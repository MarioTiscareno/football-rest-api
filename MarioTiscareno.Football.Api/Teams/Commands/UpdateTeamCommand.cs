using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using System.Text.Json.Serialization;

namespace MarioTiscareno.Football.Api.Teams.Commands;

public record UpdateTeamCommand(string Name, string Country, string League) : IRequest<Unit>
{
    [JsonIgnore]
    public string Id { get; init; } = string.Empty;
}

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Unit>
{
    private readonly ITeamDb db;

    public UpdateTeamCommandHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        UpdateTeamCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Update(new Team(request.Id, request.Name, request.Country, request.League));

        return Task.FromResult(result);
    }
}

public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.League).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
