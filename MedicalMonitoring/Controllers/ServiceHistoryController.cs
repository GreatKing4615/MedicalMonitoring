﻿using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedicalMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceHistoryController : ControllerBase
    {
        private readonly IServiceHistoryService _serviceHistoryService;

        public ServiceHistoryController(IServiceHistoryService serviceHistoryService)
        {
            _serviceHistoryService = serviceHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var history = await _serviceHistoryService.GetServiceHistoryAsync(pageNumber, pageSize);
            return Ok(history);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceHistoryById(int id)
        {
            try
            {
                var history = await _serviceHistoryService.GetServiceHistoryByIdAsync(id);
                return Ok(history);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
