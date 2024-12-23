using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using System.Text.Json.Serialization;

namespace MarioTiscareno.Football.Api.Players.Commands;

public record PatchPlayerCommand(string? Name, int? HeightInCm, int? Age, string? Nationality)
    : IRequest<Unit>
{
    [JsonIgnore]
    public string Id { get; init; } = string.Empty;
};

public class PatchPlayerCommandHandler : IRequestHandler<PatchPlayerCommand, Unit>
{
    private readonly IPlayerDb db;

    public PatchPlayerCommandHandler(IPlayerDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        PatchPlayerCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Get(request.Id)
            .Map(p =>
            {
                return p with
                {
                    Name = request.Name ?? p.Name,
                    HeightInCm = request.HeightInCm ?? p.HeightInCm,
                    Age = request.Age ?? p.Age,
                    Nationality = request.Nationality ?? p.Nationality
                };
            })
            .Bind(p => db.Update(p));

        return Task.FromResult(result);
    }
}

public class PatchPlayerCommandValidator : AbstractValidator<PatchPlayerCommand>
{
    public PatchPlayerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(100).When(x => x.Name != null);
        RuleFor(x => x.HeightInCm).GreaterThan(0).When(x => x.HeightInCm != null);
        RuleFor(x => x.Age).GreaterThan(0).When(x => x.Age != null);
        RuleFor(x => x.Nationality)
            .MinimumLength(3)
            .MaximumLength(100)
            .When(x => x.Nationality != null);
    }
}
