namespace BeerMachineApi.Services;

using System.Collections;
using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineCommands
{
    protected void StartBatch(OpcClient opcClient, BatchQueue batchQueue)
    {
        WriteBatchToServer(opcClient, batchQueue.Dequeue());

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