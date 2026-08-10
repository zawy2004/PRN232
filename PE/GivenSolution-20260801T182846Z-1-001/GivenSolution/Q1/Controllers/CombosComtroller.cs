using Microsoft.AspNetCore.Mvc;
using Q1.Models;
using Q1.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Q1.Controllers
{
    [Route("api/Combos")]
    [ApiController]
    public class CombosComtroller : Controller
    {
        private readonly Prn232PeSu2611Context _context;
        public CombosComtroller (Prn232PeSu2611Context context)
        {
                _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CombosListDto>>> GetSections()
        {
            var result = await _context.Combos
                .Select(s => new CombosListDto
                {
                    ComboId = s.ComboId,
                    ComboName = s.ComboName,
                    Price = s.Price,
                    PriceWithTax = s.Price * 105/100,
                    
                })
                .ToListAsync();

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<CreateComboRequest>> CreateSkill([FromBody] CreateComboRequest dto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.ComboName))
            {
                return BadRequest("Combo name is required");
            }

        

            bool exists = await _context.Combos
                .AnyAsync(s => s.ComboName.ToLower() == dto.ComboName.ToLower());

            if (exists)
            {
                return BadRequest("Combo name already exists");
            }

            var combo = new Combo
            {
                ComboName = dto.ComboName,
                Price = dto.Price,
                
            };

            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

          

            // 201 Created
            return CreatedAtAction(null, combo);
        }





    }
}
