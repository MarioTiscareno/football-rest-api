using System.Diagnostics;

namespace MarioTiscareno.Football.Api.Core;

public static class ResultOfExtensions
{
    public static async Task<ResultOf<T1>> MapAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, Task<T1>> map
    )
    {
        var result = await self;

        return result.IsError ? result.AsError : await map(result.AsValue);
    }

    public static async Task<ResultOf<T1>> MapAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, T1> map
    )
    {
        var result = await self;

        return result.Map(map);
    }

    public static async Task<ResultOf<T1>> BindAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, ResultOf<T1>> bind
    )
    {
        var result = await self;

        return result.IsError ? result.AsError : bind(result.AsValue);
    }

    public static async Task<ResultOf<T1>> BindAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, Task<ResultOf<T1>>> bind
    )
    {
        var result = await self;

        return result.IsError ? result.AsError : await bind(result.AsValue);
    }

    public static async Task<ResultOf<T>> ToResultAsync<T>(this Task<T> self)
    {
        var result = await self;

        Debug.Assert(
            result is not Error,
            "Result value should not be error, use 'ToResultAsync<T>(this Error self)' instead"
        );

        return result;
    }

    public static Task<ResultOf<T>> ToResultAsync<T>(this ResultOf<T> self) =>
        Task.FromResult(self);

    public static Task<ResultOf<T>> ToResultAsync<T>(this Error self) =>
        Task.FromResult(new ResultOf<T>(self));

    public static async Task<T1> MatchAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, T1> success,
        Func<Error, T1> error
    )
    {
        var result = await self;

        if (result.IsError)
        {
            return error(result.AsError);
        }

        return success(result.AsValue);
    }

    public static async Task<T1> MatchAsync<T0, T1>(
        this Task<ResultOf<T0>> self,
        Func<T0, Task<T1>> success,
        Func<Error, T1> error
    )
    {
        var result = await self;

        if (result.IsError)
        {
            return error(result.AsError);
        }

        return await success(result.AsValue);
    }

    public static async Task MatchAsync<T0>(
        this Task<ResultOf<T0>> self,
        Action<T0> success,
        Action<Error> error
    )
    {
        var result = await self;

        if (result.IsError)
        {
            error(result.AsError);
            return;
        }

        success(result.AsValue);
    }
}
