using BeerMachineApi.Services;
using Moq;
using NUnit.Framework;

using BeerMachineApi.Services;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;

namespace BeerMachineApi.Tests.Service;

public class ServiceTests
{
    private BeerMachineService _machineService;

    [SetUp]
    public void Setup()
    {
        var mockStatusModel = new BeerMachineStatusModel();
        var mockBatchStatusModel = new BatchStatusModel();
        var mockInventoryStatusModel = new InventoryStatusModel();

        // Mock is used to create a mock instance that only has "fake implementations", so we wont make database calls
        var mockBatchHandler = new Mock<IBatchHandler>().Object;
        var mockTimeHandler = new Mock<ITimeHandler>().Object;
        _machineService = new BeerMachineService(mockStatusModel, mockBatchStatusModel, mockInventoryStatusModel, mockBatchHandler, mockTimeHandler);
    }

    [Category("Service")]
    [Test]
    public void ConnectToMachine()
    {
        bool connected = false;

        Thread machineThread = new Thread(_machineService.Start);
        machineThread.Start();

        Thread.Sleep(500);

        if (_machineService.OpcClient == null)
            Assert.Fail("OpcClient is null");

        _machineService.OpcClient.Connected += (sender, e) =>
        {
            connected = true;
        };

        Thread connectToServerThread = new Thread(_machineService.TryToConnectToServer);
        connectToServerThread.Start();

        // Try to connect a couple times
        for (int i = 0; i < 4; i++)
        {
            if (connected)
                Assert.Pass();
            Thread.Sleep(500);
        }
        Assert.Fail("Connecting failed 5 times in a row, is simulation running?");
    }

    [Category("test")]
    [Test]
    public void CommandQueue()
    {
        Command[] commands = [
            new Command () { Type = "start"},
            new Command () { Type = "stop"},
            new Command () { Type = "reset"},
        ];

        foreach (Command command in commands)
        {
            _machineService.QueueCommand(command);
        }

        // Assert
        Assert.That(_machineService.CommandQueue.Count, Is.EqualTo(3));
        Assert.That(_machineService.CommandQueue.ElementAt(0).Type, Is.EqualTo("start"));
        Assert.That(_machineService.CommandQueue.ElementAt(1).Type, Is.EqualTo("stop"));
        Assert.That(_machineService.CommandQueue.ElementAt(2).Type, Is.EqualTo("reset"));
    }

    [Category("test")]
    [Test]
    public void ProcessCommands()
    {
        Command[] commands = [
            new Command () { Type = "stop"},
            new Command () { Type = "reset"},
            new Command () {
                Type = "batch",
                Parameters = new Dictionary<string, int> {
                    {"amount", 50},
                    {"speed", 100},
                    {"type", 1},
                    {"user", 1}
                }
            },
        ];

        foreach (Command command in commands)
        {
            _machineService.QueueCommand(command);
        }

        Thread machineThread = new Thread(_machineService.Start);
        machineThread.Start();

        // wait to make sure that the OpcClient instance is created
        Thread.Sleep(500);

        if (_machineService.OpcClient == null)
            Assert.Fail("OpcClient is null");

        Thread connectToServerThread = new Thread(_machineService.TryToConnectToServer);
        connectToServerThread.Start();

        Thread.Sleep(3000);
        Assert.That(_machineService.CommandQueue.Count == 0, "Command queue has not been processed");
    }

    [Category("test")]
    [Test]
    public void HandleProcessChange()
    {


    }

    [TearDown]
    public void TearDown()
    {
        // if the machine was connected we disconnect to allow new connections
        if (_machineService.OpcClient != null)
        {
            _machineService.DisconnectFromServer(_machineService.OpcClient);
        }

        _machineService = null;
    }
}