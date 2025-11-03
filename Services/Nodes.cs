using Opc.UaFx.Client;

namespace BeerMachineApi.Services;

public class Nodes
{
    protected OpcNodeInfo _CmdIdNode,
    _CmdType,
    _CmdAmount,
    _MachSpeed,
    _CntrlCmd,
    _CmdChangeRequest,
    _StatusBatchId,
    _StatusCurAmount,
    _StatusHumidity,
    _StatusTemp,
    _StatusMovement,
    _StatusMachSpeed,
    _AdminDefectiveCount,
    _AdminProcessedCount,
    _AdminStopReason,
    _AdminCurType;

    public Nodes(OpcClient _opcClient)
    {
        _CmdIdNode = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.Parameter[0].Value");
        _CmdType = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.Parameter[1].Value");
        _CmdAmount = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.Parameter[2].Value");
        _MachSpeed = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.MachSpeed");
        _CntrlCmd = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.CntrlCmd");
        _CmdChangeRequest = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Command.CmdChangeRequest");

        _StatusBatchId = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.Parameter[0].Value");
        _StatusCurAmount = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.Parameter[1].Value");
        _StatusHumidity = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.Parameter[2].Value");
        _StatusTemp = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.Parameter[3].Value");
        _StatusMovement = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.Parameter[4].Value");
        _StatusMachSpeed = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Status.MachSpeed");

        _AdminDefectiveCount = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Admin.ProdDefectiveCount");
        _AdminProcessedCount = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Admin.ProdProcessedCount");
        _AdminStopReason = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Admin.StopReason.Value");
        _AdminCurType = _opcClient.BrowseNode("ns=6;s=::Program:Cube.Admin.Parameter[0].Value");
    }
}