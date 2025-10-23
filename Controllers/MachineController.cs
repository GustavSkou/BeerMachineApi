using Microsoft.AspNetCore.Mvc;
using BeerMachine;
namespace BeerMachineApi.Controllers;

[ApiController]
[Route("machinestatus")] // expose /machinestatus
public class MachineController : ControllerBase
{

    private readonly ILogger<MachineController> _logger;

    public MachineController(ILogger<MachineController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = null)]
    public IEnumerable<MachineStatusModel> Get()
    {
        return new[] {
            MachineStatusModel.Instance
        };
    }
}