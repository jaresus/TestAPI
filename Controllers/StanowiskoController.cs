using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StanowiskoController : ControllerBase
    {
        private readonly AuthenticationContext _context;

        public StanowiskoController(AuthenticationContext context)
        {
            _context = context;
        }

        // GET: api/Stanowisko
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stanowisko>>> GetStanowiska()
        {
            return await _context.Stanowiska.ToListAsync();
        }

        // GET: api/Stanowisko/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stanowisko>> GetStanowisko(int id)
        {
            var stanowisko = await _context.Stanowiska.FindAsync(id);

            if (stanowisko == null)
            {
                return NotFound();
            }

            return stanowisko;
        }

        // PUT: api/Stanowisko/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStanowisko(int id, Stanowisko stanowisko)
        {
            if (id != stanowisko.ID)
            {
                return BadRequest();
            }

            _context.Entry(stanowisko).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StanowiskoExists(id))
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

        // POST: api/Stanowisko
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Stanowisko>> PostStanowisko(Stanowisko stanowisko)
        {
            _context.Stanowiska.Add(stanowisko);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStanowisko", new { id = stanowisko.ID }, stanowisko);
        }

        // DELETE: api/Stanowisko/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stanowisko>> DeleteStanowisko(int id)
        {
            var stanowisko = await _context.Stanowiska.FindAsync(id);
            if (stanowisko == null)
            {
                return NotFound();
            }

            _context.Stanowiska.Remove(stanowisko);
            await _context.SaveChangesAsync();

            return stanowisko;
        }

        private bool StanowiskoExists(int id)
        {
            return _context.Stanowiska.Any(e => e.ID == id);
        }
    }
}
