using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SzkoleniaController : ControllerBase
    {
        private readonly AuthenticationContext _context;

        public SzkoleniaController(AuthenticationContext context)
        {
            _context = context;
        }

        // GET: api/Szkolenia
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SzkolenieCel>>> GetSzkolenieCel()
        {
            return await _context.SzkolenieCele.ToListAsync();
        }

        // GET: api/Szkolenia/byDepartment
        [HttpGet("byDepartment")]
        public async Task<ActionResult<IEnumerable<ISzkoleniaAPI>>> GetSzkolenia([FromQuery] int[] w)
        {
            var cele = _context.SzkolenieCele.ToList();
            var wartosci = _context.Oceny
                .Include(o => o.Pracownik)
                .Where(o => o.DataDo.Value.Year == 9999 && o.Pracownik.IsActive && o.OcenaV != 0)
                .ToList();

            var wydz = _context.Wydzialy
                .Include(e => e.KwalifikacjaWydzial)
                .ThenInclude(r => r.Kwalifikacja)
                .Where(q => w.Length == 0 ? true : w.Contains(q.ID))
                .ToList();

            //uzupełnić brygady o wydzialy
            foreach (Wydzial x in wydz)
            {
                if (x.IsBrygada)
                {
                    x.KwalifikacjaWydzial = _context.KwalifikacjeWydzialy
                        .Include(k => k.Kwalifikacja)
                        .Where(k => k.WydzialID == x.IDParent)
                        .ToList();
                }
            }
            var res = wydz
                .Select(q => new ISzkoleniaAPI
                {
                    Wydzial = q,
                    Items = q.KwalifikacjaWydzial.Select(e => new IKwalSzkolCel
                    {
                        KwalifikacjaID = e.KwalifikacjaID,
                        Kwalifikacja = e.Kwalifikacja.Nazwa,
                        Cel = cele.Where(a => a.WydzialID == q.ID && a.KwalifikacjaID == e.KwalifikacjaID).Any()
                         ? cele.Where(a => a.WydzialID == q.ID && a.KwalifikacjaID == e.KwalifikacjaID).FirstOrDefault().Cel
                         : -1,
                        Wartosc = wartosci.Where(v => v.Pracownik.WydzialID == q.ID && v.KwalifikacjaID == e.KwalifikacjaID).Any()
                        ? wartosci.Where(v => v.Pracownik.WydzialID == q.ID && v.KwalifikacjaID == e.KwalifikacjaID).Count()
                        : 0
                    }).ToArray()
                })

                .ToArray();

            return res;
        }
        // GET: api/Szkolenia/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SzkolenieCel>> GetSzkolenieCel(int id)
        {
            var szkolenieCel = await _context.SzkolenieCele.FindAsync(id);

            if (szkolenieCel == null)
            {
                return NotFound();
            }

            return szkolenieCel;
        }

        // PUT: api/Szkolenia/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSzkolenieCel(int id, SzkolenieCel szkolenieCel)
        {
            if (id != szkolenieCel.ID)
            {
                return BadRequest();
            }

            _context.Entry(szkolenieCel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SzkolenieCelExists(id))
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

        // POST: api/Szkolenia
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SzkolenieCel>> PostSzkolenieCel(SzkolenieCel szkolenieCel)
        {
            var temp = _context.SzkolenieCele.Where(w =>
               w.KwalifikacjaID == szkolenieCel.KwalifikacjaID
               && w.WydzialID == szkolenieCel.WydzialID )
                .AsNoTracking()
                .ToList();
            if (!temp.Any())
            {
                _context.SzkolenieCele.Add(szkolenieCel);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetSzkolenieCel", new { id = szkolenieCel.ID }, szkolenieCel);
            }
            else
            {
                szkolenieCel.ID= temp.FirstOrDefault().ID;
                _context.Entry(szkolenieCel).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SzkolenieCelExists(szkolenieCel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return CreatedAtAction("GetSzkolenieCel", new { id = szkolenieCel.ID }, szkolenieCel);
            }

        }

        // DELETE: api/Szkolenia/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SzkolenieCel>> DeleteSzkolenieCel(int id)
        {
            var szkolenieCel = await _context.SzkolenieCele.FindAsync(id);
            if (szkolenieCel == null)
            {
                return NotFound();
            }

            _context.SzkolenieCele.Remove(szkolenieCel);
            await _context.SaveChangesAsync();

            return szkolenieCel;
        }

        private bool SzkolenieCelExists(int id)
        {
            return _context.SzkolenieCele.Any(e => e.ID == id);
        }
    }
}
