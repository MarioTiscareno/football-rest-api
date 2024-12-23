using FluentValidation;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Teams.Commands;

public record DeleteTeamCommand(string Id) : IRequest<Unit>;

public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly ITeamDb db;

    public DeleteTeamCommandHandler(ITeamDb db)
    {
        this.db = db;
    }

    public Task<ResultOf<Unit>> HandleAsync(
        DeleteTeamCommand request,
        CancellationToken cancellationToken = default
    )
    {
        return db.Delete(request.Id).ToResultAsync();
    }
}

public class DeleteTeamCommandValidator : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}
