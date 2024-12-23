using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Players.Commands;

public record DeletePlayerCommand(string Id) : IRequest<Unit>;

public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, Unit>
{
    private readonly IPlayerDb db;

    public DeletePlayerCommandHandler(IPlayerDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        DeletePlayerCommand request,
        CancellationToken cancellationToken = default
    )
    {
        return db.Delete(request.Id).ToResultAsync();
    }
}

public class DeletePlayerCommandValidator : AbstractValidator<DeletePlayerCommand>
{
    public DeletePlayerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
