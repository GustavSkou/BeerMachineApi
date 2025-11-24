using Microsoft.AspNetCore.Mvc;
using BeerMachineApi.Services;
namespace BeerMachineApi.Controllers;

[ApiController]
[Route("machine")] // expose /machine
public class MachineController : ControllerBase
{

    private readonly ILogger<MachineController> _logger;
    private IMachineService _machineHandler;

    public MachineController(IMachineService machineHandler, ILogger<MachineController> logger)
    {
        _logger = logger;
        _machineHandler = machineHandler;
    }

    [HttpGet("/status/machine")]
    public ActionResult<object> GetStatusMachine()
    {
        return _machineHandler.GetStatus("machine");
    }

    [HttpGet("/status/batch")]
    public ActionResult<object> GetStatusBatch()
    {
        return _machineHandler.GetStatus("batch");

    }

    [HttpGet("/status/inventory")]
    public ActionResult<object> GetStatusInventory()
    {
        return _machineHandler.GetStatus("inventory");
    }

    [HttpGet("/status/queue")]
    public ActionResult<object> GetStatusQueue()
    {
        return _machineHandler.GetStatus("queue");
    }

    [HttpPost("command")]
    public IActionResult PostCommand([FromBody] Command command)
    {
        if (command.Type == null || command.Type == string.Empty)
        {
            _logger.LogError("Post request with no Type");
            return BadRequest("Command type cannot be null or empty");
        }
        try
        {
            _machineHandler.ExecuteCommand(command);
            return Ok($"Executed: {command.Type}");
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError(ex, "Error executing command");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command");
            return StatusCode(500, ex.Message);
        }
    }
}
