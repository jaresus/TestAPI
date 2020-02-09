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
    public class PracownicyController : ControllerBase
    {
        private readonly AuthenticationContext context;

        public PracownicyController(AuthenticationContext context)
        {
            this.context = context;
        }

        // GET: api/Pracownicy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pracownik>>> GetPracownicy()
        {
            return await context.Pracownicy
                //.Include(p => p.Wydzial.Nazwa)
                .ToListAsync();
        }

        // GET: api/Pracownicy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pracownik>> GetPracownik(int id)
        {
            var pracownik = await context.Pracownicy.FindAsync(id);

            if (pracownik == null)
            {
                return NotFound();
            }

            return pracownik;
        }

        // PUT: api/Pracownicy/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPracownik(int id, Pracownik pracownik)
        {
            if (id != pracownik.ID)
            {
                return BadRequest();
            }

            context.Entry(pracownik).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PracownikExists(id))
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

        // POST: api/Pracownicy
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Pracownik>> PostPracownik(Pracownik pracownik)
        {
            context.Pracownicy.Add(pracownik);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPracownik", new { id = pracownik.ID }, pracownik);
        }

        // DELETE: api/Pracownicy/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pracownik>> DeletePracownik(int id)
        {
            var pracownik = await context.Pracownicy.FindAsync(id);
            if (pracownik == null)
            {
                return NotFound();
            }

            context.Pracownicy.Remove(pracownik);
            await context.SaveChangesAsync();

            return pracownik;
        }

        private bool PracownikExists(int id)
        {
            return context.Pracownicy.Any(e => e.ID == id);
        }
    }
}
