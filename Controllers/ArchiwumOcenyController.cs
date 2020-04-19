using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Kierownik")]
    public class ArchiwumOcenyController : ControllerBase
    {
        private readonly AuthenticationContext _context;

        public ArchiwumOcenyController(AuthenticationContext context)
        {
            _context = context;
        }

        // GET: api/ArchiwumOceny/query/?
        [HttpGet("query")]
        [Authorize(Roles = "Admin,Kierownik,Brygadzista")]
        public async Task<ActionResult<ArchiwumRekordAPI>> GeArchiwum([FromQuery] int page = 1,
                                                                      [FromQuery] int pageSize = 100)
        {
            var oceny =new ArchiwumRekordAPI();
            if (page==0) { page=1; }
            oceny.RowsCount = await _context.OcenaArchiwum.CountAsync();
            oceny.Items = await _context.OcenaArchiwum
                .OrderByDescending(q => q.DataUsuniecia)
                .Skip(( page-1 )*pageSize)
                .Take(pageSize)
                .ToListAsync();
            return oceny;
        }

        // GET: api/ArchiwumOceny/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Kierownik,Brygadzista")]
        public async Task<ActionResult<OcenaArchiwum>> GetOcenaArchiwum(int id)
        {
            var ocenaArchiwum = await _context.OcenaArchiwum.FindAsync(id);

            if (ocenaArchiwum == null)
            {
                return NotFound();
            }

            return ocenaArchiwum;
        }

        #region PUT: api/ArchiwumOceny/5 - niepotrzebne
        // PUT: api/ArchiwumOceny/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Kierownik")]
        //public async Task<IActionResult> PutOcenaArchiwum(int id, OcenaArchiwum ocenaArchiwum)
        //{
        //    if (id != ocenaArchiwum.ID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(ocenaArchiwum).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OcenaArchiwumExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        #endregion

        #region POST: api/ArchiwumOceny - niepotrzebne
        // POST: api/ArchiwumOceny
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost]
        //public async Task<ActionResult<OcenaArchiwum>> PostOcenaArchiwum(OcenaArchiwum ocenaArchiwum)
        //{
        //    _context.OcenaArchiwum.Add(ocenaArchiwum);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetOcenaArchiwum", new { id = ocenaArchiwum.ID }, ocenaArchiwum);
        //}
        #endregion

        #region DELETE: api/ArchiwumOceny/5 - niepotrzebne
        // DELETE: api/ArchiwumOceny/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<OcenaArchiwum>> DeleteOcenaArchiwum(int id)
        //{
        //    var ocenaArchiwum = await _context.OcenaArchiwum.FindAsync(id);
        //    if (ocenaArchiwum == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.OcenaArchiwum.Remove(ocenaArchiwum);
        //    await _context.SaveChangesAsync();

        //    return ocenaArchiwum;
        //}
        #endregion

        #region nieoptrzebne
        //private bool OcenaArchiwumExists(int id)
        //{
        //    return _context.OcenaArchiwum.Any(e => e.ID == id);
        //}
        #endregion
    }
}
