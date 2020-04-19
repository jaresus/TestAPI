﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class KwalifikacjeController : ControllerBase
    {
        private readonly AuthenticationContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public KwalifikacjeController(AuthenticationContext context,
                                      UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager=userManager;
        }

        // GET: api/Kwalifikacje
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kwalifikacja>>> GetKwalifikacje()
        {
            return await context.Kwalifikacje
                .Include(k => k.KwalifikacjaWydzial)
                .ThenInclude(w => w.Wydzial)
                .ToListAsync();
        }

        // GET: api/Kwalifikacje/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kwalifikacja>> GetKwalifikacja(int id)
        {
            var kwalifikacja = await context.Kwalifikacje
                .FindAsync(id);

            if (kwalifikacja == null)
            {
                return NotFound();
            }
            await context.Entry(kwalifikacja).Collection(c => c.KwalifikacjaWydzial).LoadAsync();
            foreach (var w in kwalifikacja.KwalifikacjaWydzial)
            {
                await context.Entry(w).Reference(r => r.Wydzial).LoadAsync();

            }
            return kwalifikacja;
        }

        // PUT: api/Kwalifikacje/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> PutKwalifikacja(int id, Kwalifikacja kwalifikacja)
        {
            if (id != kwalifikacja.ID)
            {
                return BadRequest();
            }
            context.Entry(kwalifikacja).State = EntityState.Modified;


            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KwalifikacjaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //usuń poprzednie
            var usun = context.KwalifikacjeWydzialy.Where(kw => kw.KwalifikacjaID == kwalifikacja.ID);
            context.KwalifikacjeWydzialy.RemoveRange(usun);
            context.SaveChanges();
            // dodaj nowe
            var kWydzial = kwalifikacja.KwalifikacjaWydzial.Select(r => r.WydzialID).ToArray();
            var kwalifikacjaWydzial = context.Wydzialy.Where(w => kWydzial.Contains(w.ID))
                .Select(q => new KwalifikacjaWydzial
                {
                    WydzialID = q.ID,
                    KwalifikacjaID=kwalifikacja.ID,
                    Wydzial = q
                }).ToArray();
            context.KwalifikacjeWydzialy.AddRange(kwalifikacjaWydzial);
            await context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Kwalifikacje
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<ActionResult<Kwalifikacja>> PostKwalifikacja(Kwalifikacja kwalifikacja)
        {
            var kWydzial = kwalifikacja.KwalifikacjaWydzial.Select(r => r.WydzialID).ToArray();
            kwalifikacja.KwalifikacjaWydzial = null;
            var k = context.Kwalifikacje.Add(kwalifikacja);
            await context.SaveChangesAsync();
            var kwalifikacjaWydzial = context.Wydzialy.Where(w => kWydzial.Contains(w.ID))
                .Select(q => new KwalifikacjaWydzial
                {
                    WydzialID = q.ID,
                    KwalifikacjaID=kwalifikacja.ID,
                    Wydzial = q
                });
            context.KwalifikacjeWydzialy.AddRange(kwalifikacjaWydzial);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetKwalifikacja", new { id = kwalifikacja.ID }, kwalifikacja);
        }

        // DELETE: api/Kwalifikacje/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<ActionResult<Kwalifikacja>> DeleteKwalifikacja(int id)
        {
            var kwalifikacja = await context.Kwalifikacje.FindAsync(id);
            if (kwalifikacja == null)
            {
                return NotFound();
            }
            //usunięcie ocen, jeśli istnieją oceny dla kwalifikacji to najpier je usuń
            var oceny = context.Oceny
                .Where(o => o.KwalifikacjaID == id)
                .Include(k => k.Kwalifikacja)
                .Include(p => p.Pracownik)
                .AsEnumerable();
            if (oceny != null)
            {
                string idUser = User.Claims.First(c => c.Type == "UserID").Value;
                var userL = await userManager.FindByIdAsync(idUser);
                var ocenyArchiwum = oceny.Select(o => new OcenaArchiwum
                {
                    DataDo = o.DataDo.Value,
                    DataOd = o.DataOd,
                    ID = 0,
                    Komentarz = o.Komentarz,
                    KwalifikacjaID = o.KwalifikacjaID,
                    OcenaV = o.OcenaV,
                    PracownikID = o.PracownikID,
                    StempelCzasu = o.StempelCzasu,
                    WprowadzajacyID = o.WprowadzajacyID,
                    UsuniecieKomentarz = "",
                    DataUsuniecia = DateTime.Now,
                    Kwalifikacja = o.Kwalifikacja.Nazwa,
                    Pracownik = o.Pracownik.FullName,
                    UsuwajacyID = idUser,
                    UsuwajacyNazwa = userL.FullName,
                    //Wprowadzajacy = userManager.FindByIdAsync(o.WprowadzajacyID).Result.FullName
                }).AsEnumerable().ToList();
                //var temp = ocenyArchiwum.ToList();
                foreach (var o in ocenyArchiwum)
                {
                    string fName ="";
                    try
                    { fName = userManager.FindByIdAsync(o.WprowadzajacyID).Result.FullName; }
                    catch
                    { fName = "nie istnieje"; }
                    finally
                    { o.Wprowadzajacy = fName; }
                }

                context.OcenaArchiwum.AddRange(ocenyArchiwum);
                await context.SaveChangesAsync();
                context.Oceny.RemoveRange(oceny);
                await context.SaveChangesAsync();
            }
            context.Kwalifikacje.Remove(kwalifikacja);
            await context.SaveChangesAsync();

            return kwalifikacja;
        }

        private bool KwalifikacjaExists(int id)
        {
            return context.Kwalifikacje.Any(e => e.ID == id);
        }
    }
}
