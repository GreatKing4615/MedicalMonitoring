// Controllers/PredictionController.cs

using BL.Services;
using DAL.Entities;
using DAL.Enums;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;
    private readonly PythonPredictionService _pythonPredictionService;
    private readonly IEquipmentLoadForecastRepository _equipmentLoadForecastRepository;


    public PredictionController(IPredictionService predictionService, IEquipmentLoadForecastRepository equipmentLoadForecastRepository)
    {
        _predictionService = predictionService;
        _equipmentLoadForecastRepository = equipmentLoadForecastRepository;
    }

    [HttpGet("equipment-load-arima")]
    public async Task<IActionResult> GetEquipmentLoadPredictionWithArima(
    [FromQuery] int horizon = 7,
    [FromQuery] DateTimeOffset? fromDate = null)
    {
        try
        {
            var predictions = await _predictionService.PredictEquipmentLoadForAllDeviceTypesAsync(horizon, fromDate);
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
