using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatrycaKwalifikacji.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestAPI.Models;
using TestAPI.ViewModel;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WydzialyController : ControllerBase
    {
        private readonly AuthenticationContext _context;

        public WydzialyController(AuthenticationContext context)
        {
            _context = context;
        }

        // GET: api/Wydzialy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WydzialVM>>> GetWydzialy()
        {
            //return await _context.Wydzialy.ToListAsync();
            return await _context.Wydzialy.Select(selector: x => 
                new WydzialVM { 
                    ID = x.ID, 
                    IDParent = x.IDParent, 
                    Nazwa = x.Nazwa,
                    IsBrygada = x.IsBrygada,
                    Position = x.Position 
                }).ToListAsync();
        }
        
        
        // GET: api/Wydzialy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wydzial>> GetWydzial(int id)
        {
            var wydzial = await _context.Wydzialy.FindAsync(id);

            if (wydzial == null)
            {
                return NotFound();
            }

            return wydzial;
        }

        // PUT: api/Wydzialy/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWydzial(int id, Wydzial wydzial)
        {
            if (id != wydzial.ID)
            {
                return BadRequest();
            }

            _context.Entry(wydzial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WydzialExists(id))
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

        // POST: api/Wydzialy
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Wydzial>> PostWydzial(Wydzial wydzial)
        {
            _context.Wydzialy.Add(wydzial);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWydzial", new { id = wydzial.ID }, wydzial);
        }

        // DELETE: api/Wydzialy/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Wydzial>> DeleteWydzial(int id)
        {
            var wydzial = await _context.Wydzialy.FindAsync(id);
            if (wydzial == null)
            {
                return NotFound();
            }

            _context.Wydzialy.Remove(wydzial);
            await _context.SaveChangesAsync();

            return wydzial;
        }

        private bool WydzialExists(int id)
        {
            return _context.Wydzialy.Any(e => e.ID == id);
        }
    }
}
