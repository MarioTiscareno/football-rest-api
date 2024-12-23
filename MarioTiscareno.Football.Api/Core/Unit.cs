namespace MarioTiscareno.Football.Api.Core;

/// <summary>
/// Used to represent a value-less results
/// </summary>
public readonly struct Unit
{
    public static Unit Value => new Unit();

    public static ResultOf<Unit> CreateResult() => new ResultOf<Unit>(Unit.Value);
}
