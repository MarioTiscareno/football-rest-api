using FluentValidation;

namespace MarioTiscareno.Football.Api.Core;

public class RequestPipeline
{
    private readonly IServiceProvider sp;

    private readonly ILogger logger;

    public RequestPipeline(IServiceProvider sp, ILogger<RequestPipeline> logger)
    {
        this.sp = sp;
        this.logger = logger;
    }

    public async Task<ResultOf<TResponse>> RunAsync<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<ResultOf<TResponse>>> handler,
        CancellationToken ct = default
    )
        where TRequest : IRequest<TResponse>
    {
        logger.LogInformation("Processing request {@Request}", request);

        var validator = sp.GetService<IValidator<TRequest>>();

        if (validator is not null)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed {@ValidationErrors}", validationResult.Errors);

                return new ValidationError(
                    validationResult
                        .Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                );
            }
        }

        var result = await handler(request, ct);

        logger.LogInformation(
            "Finished processing request {@Request} with result {@Result}",
            request,
            result
        );

        return result;
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
