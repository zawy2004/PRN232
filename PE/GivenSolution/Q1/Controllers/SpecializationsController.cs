using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/Specializations")]
    [ApiController]
    public class SpecializationsController : Controller
    {
        private readonly Prn232PeSu2613Context _context;
        public SpecializationsController(Prn232PeSu2613Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SecializationDto>>> GetSections()
        {
            var result = await _context.Specializations
                .Select(s => new SecializationDto
                {
                   SpecId = s.SpecId,
                   SpecName = s.SpecName,
                   Description = s.Description,
                   TrainerCount = s.Trainers.Count(),

                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
