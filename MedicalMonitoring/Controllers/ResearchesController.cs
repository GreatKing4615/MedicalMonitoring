using BL.Dtos;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedicalMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResearchesController : ControllerBase
    {
        private readonly IResearchService _researchService;

        public ResearchesController(IResearchService researchService)
        {
            _researchService = researchService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateResearch([FromBody] ResearchDto researchDto)
        {
            try
            {
                var createdResearch = await _researchService.CreateResearchAsync(researchDto);
                return CreatedAtAction(nameof(GetResearchById), new { id = createdResearch.Id }, createdResearch);
            }
            catch (ValidationException ex)
        {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetResearchById(int id)
        {
            try
            {
                var research = await _researchService.GetResearchByIdAsync(id);
                return Ok(research);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
