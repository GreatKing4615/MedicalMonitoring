// Controllers/PredictionController.cs

using BL.Services;
using DAL.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;
    private readonly PythonPredictionService _pythonPredictionService;


    public PredictionController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpGet("{deviceType}")]
    public async Task<IActionResult> GetPatientFlowPrediction(
        DeviceType deviceType,
        [FromQuery] int horizon = 7,
        [FromQuery] DateTimeOffset? fromDate = null)
    {
        try
        {
            var prediction = await _predictionService.PredictPatientFlowAsync(deviceType, horizon, fromDate);
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

    [HttpGet("{deviceType}/equipment-load-arima")]
    public async Task<IActionResult> GetEquipmentLoadPredictionWithArima(
        DeviceType deviceType,
        [FromQuery] int horizon = 7,
        [FromQuery] DateTimeOffset? fromDate = null)
    {
        try
        {
            var predictions = await _predictionService.PredictEquipmentLoadWithArimaAsync(deviceType, horizon, fromDate);
            return Ok(predictions);
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
