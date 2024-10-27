// Controllers/DataGenerationController.cs

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataGenerationController : ControllerBase
{
    private readonly IDataGenerationService _dataGenerationService;

    public DataGenerationController(IDataGenerationService dataGenerationService)
    {
        _dataGenerationService = dataGenerationService;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateData(
        [FromQuery] DateTimeOffset fromDate,
        [FromQuery] DateTimeOffset toDate,
        [FromQuery] int simulationsPerDay = 10)
    {
        try
        {
            await _dataGenerationService.GenerateSyntheticDataAsync(fromDate, toDate, simulationsPerDay);
            return Ok("Данные успешно сгенерированы.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }
}
