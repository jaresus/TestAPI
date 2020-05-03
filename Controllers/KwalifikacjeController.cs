using System;
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
        public ActionResult<IEnumerable<IKwalifikacjaAPI>> GetKwalifikacje()
        {
            var kwal = context.Kwalifikacje
                .Include(k => k.KwalifikacjaWydzial)
                .ThenInclude(w => w.Wydzial)
                .Include(k => k.KwalifikacjaStanowisko)
                .ThenInclude(s => s.Stanowisko)
                .AsEnumerable();

            return kwal.Select(k => new IKwalifikacjaAPI
            {
                ID=k.ID,
                Link=k.Link,
                Nazwa=k.Nazwa,
                Opis=k.Opis,
                Stanowisko = k.KwalifikacjaStanowisko.Select(s => s.Stanowisko).ToList(),
                Wydzial = k.KwalifikacjaWydzial.Select(w => w.Wydzial).ToList()
            }).ToList();
        }

        // GET: api/Kwalifikacje/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IKwalifikacjaAPI>> GetKwalifikacja(int id)
        {
            var kwalifikacja = await context.Kwalifikacje
                .FindAsync(id);

            if (kwalifikacja == null)
            {
                return NotFound(new { message = $"Nie znaleziono kwalifikacji o ID: {id}" });
            }
            await context.Entry(kwalifikacja).Collection(c => c.KwalifikacjaWydzial).LoadAsync();
            foreach (var w in kwalifikacja.KwalifikacjaWydzial)
            {
                await context.Entry(w).Reference(r => r.Wydzial).LoadAsync();

            }
            await context.Entry(kwalifikacja).Collection(s => s.KwalifikacjaStanowisko).LoadAsync();
            foreach (var s in kwalifikacja.KwalifikacjaStanowisko)
            {
                await context.Entry(s).Reference(r => r.Stanowisko).LoadAsync();
            }

            return new IKwalifikacjaAPI
            {
                ID = kwalifikacja.ID,
                Link = kwalifikacja.Link,
                Nazwa = kwalifikacja.Nazwa,
                Opis = kwalifikacja.Opis,
                Stanowisko = kwalifikacja.KwalifikacjaStanowisko.Select(s => s.Stanowisko).ToList(),
                Wydzial = kwalifikacja.KwalifikacjaWydzial.Select(w => w.Wydzial).ToList()
            };
        }

        // PUT: api/Kwalifikacje/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<IActionResult> PutKwalifikacja(int id, IKwalifikacjaAPI model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }
            Kwalifikacja modelKonwersja = new Kwalifikacja
            {
                ID = model.ID,
                Link = model.Link,
                Nazwa = model.Nazwa,
                Opis = model.Opis,
            };
            context.Entry(modelKonwersja).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KwalifikacjaExists(id))
                {
                    return NotFound(new { message = $"Nie znaleziono ID: {model.ID.ToString()}" });
                }
                else
                {
                    throw;
                }
            }
            //usuń wszystkie poprzednie
            var usun = context.KwalifikacjeWydzialy.Where(kw => kw.KwalifikacjaID == model.ID);
            context.KwalifikacjeWydzialy.RemoveRange(usun);
            context.SaveChanges();
            // dodaj nowe wszystkie
            var kWydzial = model.Wydzial.Select(r => r.ID).ToArray();
            var kwalifikacjaWydzial = context.Wydzialy.Where(w => kWydzial.Contains(w.ID))
                .Select(q => new KwalifikacjaWydzial
                {
                    WydzialID = q.ID,
                    KwalifikacjaID=model.ID,
                    Wydzial = q
                }).ToArray();
            context.KwalifikacjeWydzialy.AddRange(kwalifikacjaWydzial);
            await context.SaveChangesAsync();

            // stanowiska
            var usunStanowiska = context.KwalifikacjeStanowiska.Where(kw => kw.KwalifikacjaID == model.ID);
            context.KwalifikacjeStanowiska.RemoveRange(usunStanowiska);
            await context.SaveChangesAsync();
            //dodaj stanowiska 
            var kStan = model.Stanowisko.Select(s=> s.ID).ToArray();
            var kwalifikacjaStanowiska = context.Stanowiska.Where(s => kStan.Contains(s.ID))
                .Select(q => new KwalifikacjaStanowisko
                {
                    KwalifikacjaID = model.ID,
                    StanowiskoID = q.ID
                }).ToArray();
            context.KwalifikacjeStanowiska.AddRange(kwalifikacjaStanowiska);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Kwalifikacje
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize(Roles = "Admin,Kierownik")]
        public async Task<ActionResult<Kwalifikacja>> PostKwalifikacja(IKwalifikacjaAPI model)
        {
            Kwalifikacja kwalifikacja = new Kwalifikacja
            {
                ID = model.ID,
                Link = model.Link,
                Nazwa = model.Nazwa,
                Opis = model.Opis,
            };

            var k = context.Kwalifikacje.Add(kwalifikacja);
            await context.SaveChangesAsync();

            var kWydzial = model.Wydzial.Select(r => r.ID).ToArray();
            var kwalifikacjaWydzial = context.Wydzialy.Where(w => kWydzial.Contains(w.ID))
                .Select(q => new KwalifikacjaWydzial
                {
                    WydzialID = q.ID,
                    KwalifikacjaID=kwalifikacja.ID,
                    Wydzial = q
                });
            context.KwalifikacjeWydzialy.AddRange(kwalifikacjaWydzial);
            await context.SaveChangesAsync();

            var kStanowisko = model.Stanowisko.Select(s => s.ID).ToArray();
            var kwalifikacjaStanowisko = context.Stanowiska.Where(s => kStanowisko.Contains(s.ID))
                .Select(q => new KwalifikacjaStanowisko
                {
                    KwalifikacjaID = kwalifikacja.ID,
                    StanowiskoID = q.ID,
                    Stanowisko = q
                }).AsEnumerable();
            context.KwalifikacjeStanowiska.AddRange(kwalifikacjaStanowisko);
            await context.SaveChangesAsync();
            #region usuń
            //var kWydzial = mode.KwalifikacjaWydzial.Select(r => r.WydzialID).ToArray();
            //kwalifikacja.KwalifikacjaWydzial = null;
            //var k = context.Kwalifikacje.Add(kwalifikacja);
            //await context.SaveChangesAsync();
            //var kwalifikacjaWydzial = context.Wydzialy.Where(w => kWydzial.Contains(w.ID))
            //    .Select(q => new KwalifikacjaWydzial
            //    {
            //        WydzialID = q.ID,
            //        KwalifikacjaID=kwalifikacja.ID,
            //        Wydzial = q
            //    });
            //context.KwalifikacjeWydzialy.AddRange(kwalifikacjaWydzial);
            //await context.SaveChangesAsync();
            #endregion
            model.ID = kwalifikacja.ID;
            model.Stanowisko = kwalifikacjaStanowisko.Select(s => s.Stanowisko).ToArray();
            model.Wydzial = kwalifikacjaWydzial.Select(w => w.Wydzial).ToArray();
            return CreatedAtAction("GetKwalifikacja", new { id = kwalifikacja.ID }, model);
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
