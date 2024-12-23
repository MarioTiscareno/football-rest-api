namespace MarioTiscareno.Football.Api.Core;

/// <summary>
/// Result monad that can represent either a value or an error.
/// </summary>
/// <typeparam name="T"></typeparam>
public record ResultOf<T>
{
    private readonly T? value;

    private readonly Error? error;

    public ResultOf(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        this.value = value;
    }

    public ResultOf(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        this.error = error;
    }

    internal bool IsSuccess => error is null;

    internal bool IsError => error is not null;

    /// <summary>
    /// Unsafe access, should only be used internally
    /// </summary>
    internal Error AsError =>
        IsError ? error! : throw new InvalidOperationException("The result is not an error.");

    /// <summary>
    /// Unsafe access, should only be used internally
    /// </summary>
    internal T AsValue =>
        IsSuccess ? value! : throw new InvalidOperationException("The result is not a value.");

    public static implicit operator ResultOf<T>(Error _) => new(_);

    public static implicit operator ResultOf<T>(T _) => new(_);

    /// <summary>
    /// Monadic map
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="map"></param>
    /// <returns></returns>
    public ResultOf<T1> Map<T1>(Func<T, T1> map) => IsSuccess ? map(value!) : error!;

    /// <summary>
    /// Monadic bind
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="bind"></param>
    /// <returns></returns>
    public ResultOf<T1> Bind<T1>(Func<T, ResultOf<T1>> bind) =>
        IsSuccess ? bind(value!) : error!;

    /// <summary>
    /// Monadic match
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public T1 Match<T1>(Func<T, T1> success, Func<Error, T1> failure) =>
        IsSuccess ? success(value!) : failure(error!);

    /// <summary>
    /// Monadic match
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public void Match(Action<T> success, Action<Error> failure)
    {
        if (!IsSuccess)
        {
            failure(error!);
            return;
        }

        success(value!);
    }

    /// <summary>
    /// Allows transformation of error
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    public ResultOf<T> MapError(Func<Error, Error> map) => IsError ? map(AsError) : AsValue;
}
