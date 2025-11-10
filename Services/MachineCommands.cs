using Opc.UaFx;
using Opc.UaFx.Client;
using BeerMachineApi.Services.DTOs;
using BeerMachineApi.Repository;
using BeerMachineApi.Services.StatusModels;
namespace BeerMachineApi.Services;

public class MachineCommands
{
    protected void StartBatch(OpcClient opcClient, BatchDTO batch)
    {
        WriteBatchToServer(opcClient, batch);

        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 2),
                new(NodeIds.CmdChangeRequest, true)
        };

        opcClient.WriteNodes(commands);
    }

    protected void ResetMachine(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 1),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
    }

    protected void StopMachine(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 3),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
    }

    protected void AbortMachine(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 4),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
    }

    protected void ConnectToServer(OpcClient opcSession)
    {
        opcSession.Connect();
    }

    protected void DisconnectFromServer(OpcClient opcSession)
    {
        opcSession.Disconnect();
        opcSession.Dispose(); //Clean up in case it wasn't automatically handled
    }

    protected void WriteBatchToServer(OpcClient opcSession, BatchDTO batch)
    {
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, batch.Id),
            new OpcWriteNode(NodeIds.CmdType, batch.Type),
            new OpcWriteNode(NodeIds.CmdAmount, batch.Amount),
            new OpcWriteNode(NodeIds.CmdMachSpeed, batch.Speed)
        };
        opcSession.WriteNodes(commands);
    }
}