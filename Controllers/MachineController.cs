using Microsoft.AspNetCore.Mvc;
using BeerMachineApi.Models;
namespace BeerMachineApi.Controllers;

[ApiController]
[Route("beerMachine")] // expose /machinestatus
public class MachineController : ControllerBase
{

    private readonly ILogger<MachineController> _logger;
    private IMachineService _machineHandler;

    public MachineController(IMachineService machineHandler, ILogger<MachineController> logger)
    {
        _logger = logger;
        _machineHandler = machineHandler;
    }

    [HttpGet("status")]
    public IEnumerable<MachineStatusModel> Get()
    {
        return new[] {
            MachineStatusModel.Instance
        };
    }

    [HttpPost("command")]
    public IActionResult PostCommand([FromBody] Command command)
    {
        //
        if (command.Type == null || command.Type == string.Empty)
            return BadRequest("invalid command");

        try
        {
            _machineHandler.ExecuteCommand();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }
}
