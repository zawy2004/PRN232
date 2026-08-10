using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;
using System.Linq;

namespace Q1.Controllers
{
    [Route("api/filter-trainers")]
    [ApiController]
    public class FiltertrainersController : Controller
    {
        private readonly Prn232PeSu2613Context _context;
        public FiltertrainersController(Prn232PeSu2613Context context)
        {
            _context = context;
        }
    
         [HttpGet]
        public async Task<IActionResult> FilterTrainers(
           [FromQuery] int? specId,
           [FromQuery] int? minExperience
          )
        {
            var query = _context.Trainers.AsQueryable();

            if (specId.HasValue)
            {
                query = query.Where(m => m.Specs.Any(g => g.SpecId == specId.Value));
            }

            if (minExperience.HasValue)
            {
                query = query.Where(m => m.ExperienceYears >= minExperience);
            }

            var result = await query
                .Select(m => new filterTrainerResponse
                {
                   TrainerId= m.TrainerId,
                   TrainerName= m.TrainerName,
                   Email = m.Email,
                   ExperienceYears =m.ExperienceYears,
                })
                .ToListAsync();



            return Ok(result);
        }

    }


}
