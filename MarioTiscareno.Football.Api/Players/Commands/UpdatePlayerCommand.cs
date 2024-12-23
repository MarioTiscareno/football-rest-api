using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using System.Text.Json.Serialization;

namespace MarioTiscareno.Football.Api.Players.Commands;

public record UpdatePlayerCommand(string Name, int HeightInCm, int Age, string Nationality)
    : IRequest<Unit>
{
    [JsonIgnore]
    public string Id { get; init; } = string.Empty;
};

public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Unit>
{
    private readonly IPlayerDb db;

    public UpdatePlayerCommandHandler(IPlayerDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        UpdatePlayerCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var result = db.Update(
            new Player(
                request.Id,
                request.Name,
                request.HeightInCm,
                request.Age,
                request.Nationality
            )
        );

        return Task.FromResult(result);
    }
}

public class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
{
    public UpdatePlayerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.HeightInCm).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Age).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Nationality).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
