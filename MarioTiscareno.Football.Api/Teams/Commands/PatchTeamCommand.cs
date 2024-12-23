using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using System.Text.Json.Serialization;

namespace MarioTiscareno.Football.Api.Teams.Commands;

public record PatchTeamCommand(string? Name, string? Country, string? League) : IRequest<Unit>
{
    [JsonIgnore]
    public string Id { get; init; } = string.Empty;
};

public class PatchTeamCommandHandler : IRequestHandler<PatchTeamCommand, Unit>
{
    private readonly ITeamDb db;

    public PatchTeamCommandHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        PatchTeamCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Get(request.Id)
            .Map(p =>
            {
                return p with
                {
                    Name = request.Name ?? p.Name,
                    Country = request.Country ?? p.Country,
                    League = request.League ?? p.League
                };
            })
            .Bind(p => db.Update(p));

        return result.ToResultAsync();
    }
}

public class PatchTeamCommandValidator : AbstractValidator<PatchTeamCommand>
{
    public PatchTeamCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(100).When(x => x.Name != null);
        RuleFor(x => x.Country).MinimumLength(3).MaximumLength(100).When(x => x.Country != null);
        RuleFor(x => x.League).MinimumLength(3).MaximumLength(100).When(x => x.League != null);
    }
}
