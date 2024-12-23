namespace MarioTiscareno.Football.Api.Core;

public record NotFoundInDbError(string Collection, string Id)
    : Error($"Could not find entity with id {Id} in collection {Collection}")
{ }
