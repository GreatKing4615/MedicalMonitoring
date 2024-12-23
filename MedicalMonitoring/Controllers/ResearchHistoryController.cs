﻿using BL.Dtos;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedicalMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchHistoryController : ControllerBase
    {
        private readonly IResearchHistoryService _researchHistoryService;

        public ResearchHistoryController(IResearchHistoryService researchHistoryService)
        {
            _researchHistoryService = researchHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetResearchHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var history = await _researchHistoryService.GetResearchHistoryAsync(pageNumber, pageSize);
            return Ok(history);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResearchHistoryById(int id)
        {
            try
            {
                var history = await _researchHistoryService.GetResearchHistoryByIdAsync(id);
                return Ok(history);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateResearchHistory([FromBody] ResearchHistoryDto researchHistoryDto)
        {
            try
            {
                var createdHistory = await _researchHistoryService.CreateResearchHistoryAsync(researchHistoryDto);
                return CreatedAtAction(nameof(GetResearchHistoryById), new { id = createdHistory.Id }, createdHistory);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
