using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Players.Commands;

public record CreatePlayerCommand(string Name, int HeightInCm, int Age, string Nationality)
    : IRequest<PlayerResponse>;

public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, PlayerResponse>
{
    private readonly IPlayerDb db;

    public CreatePlayerCommandHandler(IPlayerDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<PlayerResponse>> HandleAsync(
        CreatePlayerCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var player = new Player(
            Guid.NewGuid().ToString("N"),
            request.Name,
            request.HeightInCm,
            request.Age,
            request.Nationality
        );

        var response = db.Insert(player).Map(_ => player.ToResponse());

        return Task.FromResult(response);
    }
}

public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
{
    public CreatePlayerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.HeightInCm).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Age).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Nationality).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
