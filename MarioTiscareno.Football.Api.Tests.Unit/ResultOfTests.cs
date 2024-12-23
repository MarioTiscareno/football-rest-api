using FluentAssertions;
using MarioTiscareno.Football.Api.Core;

namespace MarioTiscareno.Football.Api.Tests.Unit;

public class ResultOfTests
{
    [Fact]
    public void Result_From_Value_Matches_Success()
    {
        // Arrange
        const int value = 123;

        // Act

        // map, bind and match just for test coverage
        int result = new ResultOf<int>(value)
            .Map(o => o.ToString())
            .Bind(o => new ResultOf<int>(int.Parse(o!)))
            .Match(success: v => v, _ => 0);

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public void Result_From_Error_Matches_Failure()
    {
        // Arrange
        var error = new SimpleError("This is a simple error", -1);

        // Act

        // map, bind and match just for test coverage
        int result = new ResultOf<int>(error)
            .Map(o => o.ToString())
            .Bind(o => new ResultOf<int>(int.Parse(o!)))
            .Match(_ => 0, failure: e =>
            {
                return e switch
                {
                    SimpleError simpleError => (int)simpleError.Data,
                    _ => 0
                };
            });

        // Assert
        result.Should().Match(i => i == -1);
    }
}

public record SimpleError(string Message, object Data) : Error(Message);
