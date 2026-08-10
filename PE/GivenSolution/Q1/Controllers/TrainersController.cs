using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;
using static Azure.Core.HttpHeader;

namespace Q1.Controllers
{
    [Route("api/trainers")]
    [ApiController]
    public class TrainersController : Controller
    {
        private readonly Prn232PeSu2613Context _context;
        public TrainersController(Prn232PeSu2613Context context)
        {
            _context = context;
        }
    
     [HttpPost]
        public async Task<ActionResult<CreateTrainerRequest>> CreateTrainer([FromBody] CreateTrainerRequest dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Email is required");
            }



            bool exists = await _context.Trainers
                .AnyAsync(s => s.Email == dto.Email);

            if (exists)
            {
                return BadRequest("Trainer email already exists");
            }

            var trainer = new Trainer
            {
                TrainerName = dto.TrainerName,
                Email = dto.Email,
                ExperienceYears = dto.ExperienceYears,
            };

            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();



            // 201 Created
            return CreatedAtAction(null, trainer);
        }
        [HttpDelete("{trainerId}")]
        public async Task<IActionResult> DeleteGenres(int trainerId)
        {
            var trainer = await _context.Trainers
                .Include(g=> g.Bookings)
                .FirstOrDefaultAsync(g => g.TrainerId == trainerId);
            if (trainer == null)
            {       
                return NotFound();
            }

            if (trainer.Bookings.Any())
            {
                return BadRequest(new { message = "Cannot delete trainer with active booking records" });
            }

            _context.Trainers.Remove(trainer);

            await _context.SaveChangesAsync();

            return NoContent();
        }


    } 
}
