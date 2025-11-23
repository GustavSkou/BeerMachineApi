using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BeerMachineApi.Controllers;
using BeerMachineApi.Services;

namespace BeerMachineApi.Tests.Controllers;

public class Tests
{
    private Mock<IMachineService> _mockMachineService;
    private Mock<ILogger<MachineController>> _mockLogger;
    private MachineController _controller;

    [SetUp]
    public void Setup ()
    {
        _mockMachineService = new Mock<IMachineService> ();
        _mockLogger = new Mock<ILogger<MachineController>> ();
        _controller = new MachineController ( _mockMachineService.Object, _mockLogger.Object );
    }

    [Test]
    public void StartBatchSequence ()
    {
        // Arrange
        Command[] commands =
        [
            new Command () { Type = "stop" },
            new Command () { Type = "reset" },
            new Command () {
                Type = "batch",
                Parameters = new Dictionary<string, int>() {
                    {"id", 1},
                    {"amount", 50},
                    {"speed", 100},
                    {"type", 1},
                    {"user", 1}
                }
            },
        ];

        // Setup mock to not throw exceptions for valid commands
        _mockMachineService.Setup ( s => s.ExecuteCommand ( It.IsAny<Command> () ) ).Verifiable ();

        // Act & Assert
        foreach ( Command command in commands )
        {
            var result = _controller.PostCommand(command);

            if ( result is not OkObjectResult )
                Assert.Fail ( $"Command {command.Type} should succeed" );

            var okResult = result as OkObjectResult;

            if ( okResult?.Value?.ToString () != $"Executed: {command.Type}" )
                Assert.Fail ( "wrong message" );

        }
        Assert.Pass ();
    }
}
