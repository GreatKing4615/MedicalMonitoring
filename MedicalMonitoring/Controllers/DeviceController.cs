using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var devices = await _deviceService.GetDevicesAsync(pageNumber, pageSize);
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return Ok(device);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
