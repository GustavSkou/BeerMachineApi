namespace BeerMachineApi.Services;

using Opc.UaFx;
using Opc.UaFx.Client;

public class MachineCommands
{
    protected void StartBatch(OpcClient opcSession)
    {
        OpcWriteNode[] commands = {
                new(NodeIds.CntrlCmd, 2),
                new(NodeIds.CmdChangeRequest, true)
            };
        opcSession.WriteNodes(commands);
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

    protected void WriteBatchToServer(OpcClient opcSession, float id, float type, float amount, float speed)
    {
        OpcWriteNode[] commands = {
            new OpcWriteNode(NodeIds.CmdId, id),
            new OpcWriteNode(NodeIds.CmdType, type),
            new OpcWriteNode(NodeIds.CmdAmount, amount),
            new OpcWriteNode(NodeIds.MachSpeed, speed)
        };
        opcSession.WriteNodes(commands);
    }
}