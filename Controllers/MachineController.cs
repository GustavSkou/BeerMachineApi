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

    [HttpGet("status")] // /machine/status
    public IEnumerable<object> Get()
    {
        return [
            _machineHandler.GetStatus()
        ];
    }

    [HttpPost("command")]
    public IActionResult PostCommand([FromBody] Command command)
    {
        if (command.Type == null || command.Type == string.Empty)
            return BadRequest("invalid command");

        try
        {
            _machineHandler.ExecuteCommand(command);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }
}
