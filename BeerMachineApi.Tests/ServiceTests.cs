using BeerMachineApi.Controllers;
using BeerMachineApi.Services;
using Microsoft.AspNetCore.Mvc;
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
    public void Setup ()
    {
        var mockStatusModel = new BeerMachineStatusModel();
        var mockBatchStatusModel = new BatchStatusModel();
        var mockInventoryStatusModel = new InventoryStatusModel();
        var mockBatchHandler = new Mock<IBatchHandler>().Object;
        var mockTimeHandler = new Mock<ITimeHandler>().Object;
        _machineService = new BeerMachineService( mockStatusModel, mockBatchStatusModel, mockInventoryStatusModel, mockBatchHandler, mockTimeHandler);
    }

    [Category ( "Service" )]
    [Test]
    public void ConnectToMachine ()
    {
        Thread machineThread = new Thread(_machineService.Start);
        machineThread.Start ();

        Thread.Sleep ( 100 );

        _machineService.OpcClient.Connected += ( sender, e ) =>
        {
            Assert.Pass ();
        };

        Thread connectToServerThread = new Thread(_machineService.TryToConnectToServer);
        connectToServerThread.Start ();

        // Try to connect a couple times
        for ( int i = 0; i < 4; i++ )
        {
            Thread.Sleep ( 1000 );
        }
        Assert.Fail ( "Connecting failed 5 times in a row, is simulation running?" );      
    }

    [Category ( "test" )]
    [Test]
    public void CommandQueue ()
    {
        Command[] commands = [
            new Command () { Type = "start"},
            new Command () { Type = "stop"},
            new Command () { Type = "reset"},
        ];

        foreach ( Command command in commands )
        {
            _machineService.QueueCommand ( command );
        }
    }

    [Category ( "test" )]
    [Test]
    public void ProcessCommand ()
    {

    }

    [Category ( "test" )]
    [Test]
    public void HandleProcessChange ()
    {

    }
}