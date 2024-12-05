using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly ISimulationService _simulationService;

        public SimulationController(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [HttpPost]
        public async Task<IActionResult> RunSimulation(
            [FromQuery] int simulationDays,
            [FromQuery] int minPatientsPerDay = 50,
            [FromQuery] int maxPatientsPerDay = 100)
        {
            try
            {
                var results = await _simulationService.RunSimulationAsync(simulationDays, minPatientsPerDay, maxPatientsPerDay);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
    }
}
