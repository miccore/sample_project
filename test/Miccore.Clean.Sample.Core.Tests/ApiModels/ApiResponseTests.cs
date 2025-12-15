using System.Net;
using FluentAssertions;
using Miccore.Clean.Sample.Core.ApiModels;

namespace Miccore.Clean.Sample.Core.Tests.ApiModels;

public class ApiResponseTests
{
    [Fact]
    public void Success_ShouldReturnApiResponseWithData()
    {
        // Arrange
        var data = new TestData { Id = 1, Name = "Test" };

        // Act
        var response = ApiResponse<TestData>.Success(data);

        // Assert
        response.Data.Should().Be(data);
        response.Errors.Should().BeNull();
    }

    [Fact]
    public void Error_ShouldReturnApiResponseWithSingleErrorMessage()
    {
        // Arrange
        var httpStatus = HttpStatusCode.BadRequest;
        var message = "An error occurred";

        // Act
        var response = ApiResponse<TestData>.Error(httpStatus, message);

        // Assert
        response.Data.Should().BeNull();
        response.Errors.Should().HaveCount(1);
        response.Errors!.First().Code.Should().Be((int)httpStatus);
        response.Errors!.First().Message.Should().Be(message);
    }

    [Fact]
    public void Error_ShouldReturnApiResponseWithMultipleErrorMessages()
    {
        // Arrange
        var httpStatus = HttpStatusCode.BadRequest;
        var messages = new List<string> { "Error 1", "Error 2" };

        // Act
        var response = ApiResponse<TestData>.Error(httpStatus, messages);

        // Assert
        response.Data.Should().BeNull();
        response.Errors.Should().HaveCount(messages.Count);
        response.Errors!.Select(e => e.Message).Should().BeEquivalentTo(messages);
    }

    [Fact]
    public void Error_ShouldReturnApiResponseWithMultipleApiErrors()
    {
        // Arrange
        var errors = new List<ApiError>
        {
            new ApiError { Code = (int)HttpStatusCode.BadRequest, Message = "Error 1" },
            new ApiError { Code = (int)HttpStatusCode.InternalServerError, Message = "Error 2" }
        };

        // Act
        var response = ApiResponse<TestData>.Error(errors);

        // Assert
        response.Data.Should().BeNull();
        response.Errors.Should().BeEquivalentTo(errors);
    }

    private class TestData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}