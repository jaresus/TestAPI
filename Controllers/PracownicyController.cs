﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracownicyController : ControllerBase
    {
        private readonly AuthenticationContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<UserProfileController> logger;

        public PracownicyController(AuthenticationContext context,
                                    UserManager<ApplicationUser> userManager,
                                    ILogger<UserProfileController> logger)
        {
            this.context = context;
            this.userManager=userManager;
            this.logger=logger;
        }

        // GET: api/Pracownicy
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pracownik>>> GetPracownicy()
        {
            return await context.Pracownicy
                .Include(p => p.Wydzial)
                .Include(s => s.Stanowisko)
                .ToListAsync();
        }

        // GET: api/Pracownicy/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pracownik>> GetPracownik(int id)
        {
            var pracownik = await context.Pracownicy
                .Include(p => p.Wydzial)
                .Where(p => p.ID == id).FirstOrDefaultAsync();

            if (pracownik == null)
            {
                return NotFound();
            }
            Console.WriteLine(pracownik.ToString());
            return pracownik;
        }

        // GET: api/Pracownicy/5
        [HttpGet("byPersonalNr/{numer}")]
        public async Task<ActionResult<Pracownik>> GetPracownik(string numer)
        {
            var pracownik = await context.Pracownicy
                .Include(p => p.Wydzial)
                .Include(p => p.Stanowisko)
                .Where(p => p.NrPersonalny == numer).FirstOrDefaultAsync();

            if (pracownik == null)
            {
                return NotFound(new { message = $"nie znaleziono pracownika z numerem personalnym: {numer}"});
            }
            Console.WriteLine(pracownik.ToString());
            return pracownik;
        }

        // PUT: api/Pracownicy/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Kierownik,Brygadzista")]
        public async Task<IActionResult> PutPracownik(int id, Pracownik pracownik)
        {
            if (id != pracownik.ID)
            {
                return BadRequest();
            }
            var nrPersonalny = context.Pracownicy
                .Where(p => p.NrPersonalny == pracownik.NrPersonalny && p.ID != pracownik.ID).FirstOrDefault();
            if (nrPersonalny != null)
            {
                return BadRequest($"taki numer peronalny należy do: {nrPersonalny.FullName}");
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
        [Authorize(Roles = "Admin,Kierownik,Brygadzista")]
        public async Task<ActionResult<Pracownik>> PostPracownik(Pracownik pracownik)
        {
            pracownik.ID=0;
            var nrPersonalny = context.Pracownicy.Where(p => p.NrPersonalny == pracownik.NrPersonalny).FirstOrDefault();
            if (nrPersonalny != null)
            {
                return BadRequest($"Taki numer personalny już istnieje dla: {nrPersonalny.FullName}");
            }
            context.Pracownicy.Add(pracownik);
            await context.SaveChangesAsync();
            var wydzial = context.Wydzialy.Where(w => w.ID == pracownik.WydzialID).FirstOrDefault();
            var stanowisko = context.Stanowiska.Where(s=> s.ID == pracownik.StanowiskoID).FirstOrDefault();
            pracownik.Wydzial = wydzial;
            pracownik.Stanowisko = stanowisko;
            return CreatedAtAction("GetPracownik", new { id = pracownik.ID }, pracownik);
        }

        // DELETE: api/Pracownicy/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<ActionResult<Pracownik>> DeletePracownik(int id)
        {
            var pracownik = await context.Pracownicy.FindAsync(id);
            if (pracownik == null)
            {
                return NotFound();
            }
            logger.LogWarning("Test logów");
            //usunięcie ocen
            var oceny = context.Oceny
                .Where(o => o.PracownikID == id)
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

                try
                {
                    context.OcenaArchiwum.AddRange(ocenyArchiwum);
                    await context.SaveChangesAsync();
                }
                catch
                {
                    logger.LogError("ocena rchiwum");
                    return BadRequest(new { message = "błąd" });
                }
                context.Oceny.RemoveRange(oceny);
                await context.SaveChangesAsync();
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
