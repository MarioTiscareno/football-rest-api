using FluentValidation;

namespace MarioTiscareno.Football.Api.Core;

public class RequestPipeline
{
    private readonly IServiceProvider sp;

    public RequestPipeline(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public async Task<ResultOf<TResponse>> RunAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<ResultOf<TResponse>>> handler,
        CancellationToken ct = default
    )
        where TRequest : IRequest<TResponse>
    {
        var validator = sp.GetService<IValidator<TRequest>>();

        if (validator is not null)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return new ValidationError(
                    validationResult
                        .Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                );
            }
        }

        return await handler(request, ct);
    }
}

public record ValidationError(IDictionary<string, string[]> ErrorDetails)
    : Error("Validation failed")
{
    public IResult CreateProblemResult()
    {
        return Results.ValidationProblem(ErrorDetails);
    }
}
