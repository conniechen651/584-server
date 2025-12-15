using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using _584_server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolModel;

namespace _584_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController(SchoolDbContext context) : ControllerBase
    {

        // GET: api/Districts
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<District>>> GetDistricts()
        {
            return await context.Districts.ToListAsync();
        }
        
        [HttpGet("score")]
        public async Task<ActionResult<IEnumerable<DistrictScore>>> GetDistrictScore()
        {
            return await context.Districts
                .Select(d => new DistrictScore()
                {
                    Id = d.Id,
                    Name = d.Name,
                    County = d.County,
                    MathScore = d.Schools.Any() ? d.Schools.Average(s => s.MathScore) : 0,
                    ReadingScore = d.Schools.Any() ? d.Schools.Average(s => s.ReadingScore) : 0,
                    WritingScore = d.Schools.Any() ? d.Schools.Average(s => s.WritingScore) : 0
                })
                .ToListAsync();
        }

        [HttpGet("score/{id}")]
        public ActionResult<DistrictScore> GetDistrictScoreById(int id)
        {
            var dto = context.Districts
                .Where(d => d.Id == id)
                .Select(d => new DistrictScore
                {
                    Id = d.Id,
                    Name = d.Name,
                    County = d.County,
                    MathScore = d.Schools.Any() ? d.Schools.Average(s => s.MathScore) : 0,
                    ReadingScore = d.Schools.Any() ? d.Schools.Average(s => s.ReadingScore) : 0,
                    WritingScore = d.Schools.Any() ? d.Schools.Average(s => s.WritingScore) : 0
                })
                .SingleOrDefault();

            if (dto == null)
            {
                return NotFound();
            }

            return dto;
        }
        

        // GET: api/Districts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<District>> GetDistrict(int id)
        {
            var district = await context.Districts.FindAsync(id);

            if (district == null)
            {
                return NotFound();
            }

            return district;
        }

        // PUT: api/Districts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDistrict(int id, District district)
        {
            if (id != district.Id)
            {
                return BadRequest();
            }

            context.Entry(district).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistrictExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Districts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<District>> PostDistrict(District district)
        {
            context.Districts.Add(district);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetDistrict", new { id = district.Id }, district);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            var district = await context.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            context.Districts.Remove(district);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool DistrictExists(int id)
        {
            return context.Districts.Any(e => e.Id == id);
        }
    }
}