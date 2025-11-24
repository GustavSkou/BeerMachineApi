using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BeerMachineApi.Controllers;
using BeerMachineApi.Services;

namespace BeerMachineApi.Tests.Controllers;

public class ControllerTests
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

    [Category ( "Controller" )]
    [Category ( "Get Request" )]
    [Test]
    public void GetMachineStatus()
    {
        Dictionary<string, object> expectedStatus = new Dictionary<string, object>()
        {
            {"speed", 0},
            {"ctrlcmd", 0},
            {"temperature", 0},
            {"vibration", 0},
            {"humidity", 0},
            {"stopReason", 0},
            {"stateCurrent", 2}
        };

        _mockMachineService.Setup ( s => s.GetStatus ( "machine" ) ).Returns ( expectedStatus );
        var actionResult = _controller.GetStatusMachine ();

        ValidateStatusResult ( actionResult, expectedStatus );
    }

    [Category ( "Controller" )]
    [Category ( "Get Request" )]
    [Test]
    public void GetBatchStatus ()
    {
        Dictionary<string, object> expectedStatus = new Dictionary<string, object>()
        {
            { "batchId", null },
            { "beerType", null },
            { "speed", 0 },
            { "toProduceAmount", 0 },
            { "producedAmount", 0 },
            { "defectiveAmount", 0 },
            { "userId", 0 },
            { "failureRate", 0 }
        };

        _mockMachineService.Setup ( s => s.GetStatus ( "batch" ) ).Returns ( expectedStatus );
        var actionResult = _controller.GetStatusBatch ();

        ValidateStatusResult ( actionResult, expectedStatus );
    }
    
    [Category ( "Controller" )]
    [Category ( "Get Request" )]
    [Test]
    public void GetInventoryStatus ()
    {
        Dictionary<string, object> expectedStatus = new Dictionary<string, object>()
        {
            { "barley", 0 },
            { "hops", 0 },
            { "malt", 0 },
            { "wheat", 0 },
            { "yeast", 0 },
            { "fillingInventory", false }
        };

        _mockMachineService.Setup ( s => s.GetStatus ( "inventory" ) ).Returns ( expectedStatus );
        var actionResult = _controller.GetStatusInventory ();

        ValidateStatusResult ( actionResult, expectedStatus );
    }

    private void ValidateStatusResult ( ActionResult<object> actionResult, Dictionary<string, object> expectedStatus )
    {
        var value = actionResult.Value;
        var status = value as Dictionary<string, object>;

        if ( status is null )
        {
            Assert.Fail ( "Result is expetced to be type Dictionary<string, object>" );
            return;
        }

        if ( status.Count != status.Count )
        {
            Assert.Fail ( "The status is a different size" );
        }

        foreach ( var key in status.Keys )
        {
            if ( !expectedStatus.ContainsKey ( key ) )
                Assert.Fail ();
        }
        Assert.Pass ();
    }
    
    [Category ( "Controller" )]
    [Category ( "Post Request" )]
    [Test]
    public void StartBatchSequence ()
    {
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

        _mockMachineService.Setup ( s => s.ExecuteCommand ( It.IsAny<Command> () ) ).Verifiable ();

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
