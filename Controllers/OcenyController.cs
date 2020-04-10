using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MatrycaKwalifikacji.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using TestAPI.Models;
using System.Data;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class OcenyController : ControllerBase
    {
        private readonly AuthenticationContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public OcenyController(AuthenticationContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
     

        // GET: api/Oceny/detail/id
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<IEnumerable<IKwalifikacjaOcenyAPI>>> GetDetails(int id)
        {
            var oceny = await context.Kwalifikacje
                .Include(w =>w.KwalifikacjaWydzial)
                .ThenInclude(w => w.Wydzial)
                .Include(o => o.Oceny)
                .Select(o => new IKwalifikacjaOcenyAPI
                {
                    ID = o.ID,
                    Nazwa = o.Nazwa,
                    Wydzialy = o.KwalifikacjaWydzial.Select(w => new IWydzialKwalifikacjaAPI
                    {
                        IDw=w.WydzialID,
                        Nazwa=w.Wydzial.Nazwa
                    }).ToArray(),
                    Oceny = o.Oceny.Where(q=> q.PracownikID == id)
                     .Select(d => new IDKwalifikacjaOcenaAPI
                     {
                         ID = d.ID,
                         K = d.KwalifikacjaID,
                         O = d.OcenaV,
                         Od =d.DataOd,
                         Do = d.DataDo.Value,
                         S = d.StemelCzasu,
                         Kom = d.Komentarz,
                         WID = d.WprowadzajacyID
                     }).ToArray()
                })
                .ToListAsync();
            return oceny;
        }

        // GET: api/Oceny
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ocena>>> GetOceny()
        {
            return await context.Oceny.ToListAsync();
        }

        // GET: api/Oceny/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ocena>> GetOcena(int id)
        {
            var ocena = await context.Oceny.FindAsync(id);

            if (ocena == null)
            {
                return NotFound();
            }

            return ocena;
        }
        // GET: api/Oceny/byDepartment/?
        [HttpGet("byDepartment")]
        public async Task<ActionResult<IEnumerable<OcenaListaAPI>>> GetOcenybyDepartment([FromQuery] int[] p, [FromQuery] int[] k)
        {
            int[] pracownicy = p;
            int[] kompetencje = k;

            //lista techniczna: jakie kwalifikacje wyświetlać
            var listaKwalifikacji = context.Kwalifikacje
                .Include(q => q.KwalifikacjaWydzial)
                .Where(q => q.KwalifikacjaWydzial.Any(w => kompetencje.Contains(w.WydzialID)))
                .Select(k => k.ID)
                .ToList();
            //uwzględnij pracowników z działów zagnieżdżonych

            var oceny = await context.Pracownicy.OrderBy(p => p.Nazwisko)
                .Include(o => o.Oceny)
                .Where(p => p.IsActive)
                .Where (p => pracownicy.Contains(p.WydzialID))
                .Select(p => new OcenaListaAPI
                {
                    PracownikID = p.ID,
                    NrPersonalny = p.NrPersonalny,
                    Pracownik = p.Nazwisko + ", " + p.Imie,
                    Test = p.Oceny.Count(),
                    KwalifikacjaOcena = p.Oceny
                        .Where(o => o.DataDo.Value.Year == 9999)
                        //.Where(o => listaKwalifikacji.Contains(o.KwalifikacjaID))
                        .Select(o => new IDKwalifikacjaOcenaAPI
                        {
                            ID = o.ID,
                            K = o.KwalifikacjaID,
                            O = o.OcenaV,
                            Od = o.DataOd,
                            Do = o.DataDo.Value,
                            S = o.StemelCzasu,
                            WID = o.WprowadzajacyID,
                            Kom = o.Komentarz
                        }).ToArray()
                })
                .ToListAsync();
            if (oceny == null)
            {
                return NotFound();
            }
            Console.WriteLine("gotowe");
            return oceny;
        }

        // PUT: api/Oceny/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOcena(int id, Ocena ocena)
        {
            if (id != ocena.ID)
            {
                return BadRequest();
            }

            context.Entry(ocena).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OcenaExists(id))
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

        // POST: api/Oceny
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Ocena>> PostOcena(Ocena model)
        {

            using (var transaction = context.Database.BeginTransaction())
            {

                var ostatniaOcena = context.Oceny.Where(o =>
                        o.PracownikID == model.PracownikID &&
                        o.KwalifikacjaID == model.KwalifikacjaID &&
                        o.DataDo.Value.Year == 9999).FirstOrDefault();

                if (ostatniaOcena != null)
                {
                    if (ostatniaOcena.DataOd == model.DataOd)
                    {
                        ostatniaOcena.OcenaV = model.OcenaV;
                    }
                    else
                    {
                        ostatniaOcena.DataDo = model.DataOd.AddDays(-1);
                    }
                    context.Update(ostatniaOcena);
                    await context.SaveChangesAsync();
                }
                //użytkownik
                //string userId=User.Claims.First(c => c.Type == "UserID").Value;                        
                //model.WprowadzajacyID = userId;
                model.DataDo = new DateTime(9999, 12, 31);
                model.StemelCzasu = DateTime.Now;
                context.Oceny.Add(model);
                await context.SaveChangesAsync();
                transaction.Commit();
                //wyślij maila
                return CreatedAtAction("GetOcena", new { id = model.ID }, model);
            }
            //return BadRequest();
        }



        // DELETE: api/Oceny/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ocena>> DeleteOcena(int id)
        {
            var ocena = await context.Oceny.FindAsync(id);
            if (ocena == null)
            {
                return NotFound("Nie znaleziono");
            }

            context.Oceny.Remove(ocena);
            await context.SaveChangesAsync();
            //znajdź następną ocenę
            //var oceny = context.Oceny.Where(o =>
            //    o.KwalifikacjaID == ocena.KwalifikacjaID
            //    && o.PracownikID == ocena.PracownikID
            //  );
            //foreach (Ocena i in oceny)
            //{

            //}
            //znajdź poprzednia
            return ocena;
        }

        private bool OcenaExists(int id)
        {
            return context.Oceny.Any(e => e.ID == id);
        }

        private int[] UwzglednijPoziomyZagniezdzone(int aktualnyPoziom, bool uwzglednijBrygady)
        {
            #region przygotowanie drzewa
            List<TreeNode> _nodes = new List<TreeNode>();
            if (uwzglednijBrygady)
            {
                var wydzialy = context.Wydzialy;
                foreach (Wydzial w in wydzialy)
                {
                    _nodes.Add(new TreeNode { id=w.ID, text = w.Nazwa, parent = w.IDParent.ToString() });
                }
                Tree tree = TreeBuilder.BuildTree(_nodes);
                // dodanie podległych działów do wyniku - potomków w drzewie
                Tree y = TreeExtensions.FindNode(tree, aktualnyPoziom);
                List<Tree> x = TreeExtensions.Descendants(y).ToList();
                return x.Select(t => ( int )t.Id).ToArray();
            }
            else
            {
                var wydzialy = context.Wydzialy.Where(w => w.IsBrygada==uwzglednijBrygady);
                foreach (Wydzial w in wydzialy)
                {
                    _nodes.Add(new TreeNode { id=w.ID, text=w.Nazwa, parent = w.IDParent.ToString() });
                }
                Tree tree = TreeBuilder.BuildTree(_nodes);
                // dodanie podległych działów do wyniku - potomków w drzewie
                Tree y = TreeExtensions.FindNode(tree, aktualnyPoziom);
                List<Tree> x = TreeExtensions.Descendants(y).ToList();
                return x.Select(t => ( int )t.Id).ToArray();
            }
            #endregion

        }
    }
}
