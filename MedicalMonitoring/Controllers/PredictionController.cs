using BL.Services;
using DAL.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public PredictionController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpGet("{deviceType}")]
    public async Task<IActionResult> GetPatientFlowPrediction(DeviceType deviceType, [FromQuery] int horizon = 7)
    {
        try
        {
            var prediction = await _predictionService.PredictPatientFlowAsync(deviceType, horizon);
            return Ok(prediction.Forecasts);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }
}
