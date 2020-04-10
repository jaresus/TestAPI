using System;
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

    public class UserProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private readonly AuthenticationContext context;
        private ILogger<UserProfileController> logger;
        const string cWydzial = "Wydzial";
        const string cKwalifikacja = "Kwalifikacja";

        public UserProfileController(UserManager<ApplicationUser> userManager,
            AuthenticationContext context,
            ILogger<UserProfileController> logger)
        {
            this.userManager = userManager;
            this.context=context;
            this.logger = logger;
        }
        [HttpGet("InfoLastOperation{WprowadzajacyID}")]
        //GET : /api/UserProfile/InfoLastOperation
        public async Task<ActionResult<Object>> InfoLastOperation(string WprowadzajacyID)
        {
            var oceny = await context.Oceny.Where(o => o.WprowadzajacyID == WprowadzajacyID)
                .OrderByDescending(o => o.StemelCzasu).FirstOrDefaultAsync();
            oceny.Kwalifikacja = await context.Kwalifikacje.FirstOrDefaultAsync(k => k.ID == oceny.KwalifikacjaID);
            oceny.Pracownik = await context.Pracownicy.FirstOrDefaultAsync(p => p.ID == oceny.PracownikID);
            return oceny;
        }

        [HttpPut("{id}")]
        //PUT : /api/UserProfile/id
        public async Task<IActionResult> PutUserProfile(string id, UserModelProfil profile)
        {
            if (id != profile.Id)
            {
                return BadRequest();
            }
            var user = await userManager.FindByIdAsync(id);
            user.FullName = profile.FullName;
            user.Email = profile.Email;
            user.UserName = profile.UserName;
            context.Entry(user).State = EntityState.Modified;

            var oldRoles = await userManager.GetRolesAsync(user);
            //dodaj brakujące ROLE
            var missRoles = profile.Role.Except(oldRoles);
            await userManager.AddToRolesAsync(user, missRoles);
            //usuń zbyteczne ROLE
            var surplusRoles = oldRoles.Except(profile.Role);
            await userManager.RemoveFromRolesAsync(user, surplusRoles);

            #region dodaj - usuń WYDZIALY
            //dodaj brakujące WYDZIAŁY
            var poczWyd1 = profile.PoczatkoweWydzialy.Select(p => p.ID).ToArray();
            var poczWyd2 = context.PoczatkoweWydzialy
                .Where(p => p.UserID == user.Id && p.Typ == cWydzial).Select(p => p.WydzialID).ToArray();
            var missWydzialy = poczWyd1.Except(poczWyd2);
            var poczWydzADD = missWydzialy
                .Select(p => new PoczatkoweWydzialy
                {
                    Typ=cWydzial,
                    UserID=user.Id,
                    WydzialID = p
                });
            context.PoczatkoweWydzialy.AddRange(poczWydzADD);
            await context.SaveChangesAsync();

            //usun zbyteczne WYdzialy
            var surplusWydz = poczWyd2.Except(poczWyd1);
            var poczWydzDelete = context.PoczatkoweWydzialy.Where(p => surplusWydz.Contains(p.WydzialID) && p.Typ == cWydzial);
            context.PoczatkoweWydzialy.RemoveRange(poczWydzDelete);
            await context.SaveChangesAsync();
            #endregion

            #region dodaj - usuń KOMPETENCJE
            //dodaj brakujące KOMPETENCJE (WYDZIALY)
            var poczKwal1 = profile.PoczatkoweKwalifikacje.Select(k => k.ID).ToArray();
            var poczKwal2 = context.PoczatkoweWydzialy
                .Where(k => k.UserID == user.Id && k.Typ == cKwalifikacja).Select(k => k.WydzialID).ToArray();
            var missKwal = poczKwal1.Except(poczKwal2);
            var poczKwalAdd = missKwal
                .Select(k => new PoczatkoweWydzialy
                {
                    Typ=cKwalifikacja,
                    UserID = user.Id,
                    WydzialID= k
                });
            context.PoczatkoweWydzialy.AddRange(poczKwalAdd);
            await context.SaveChangesAsync();
            //usuń zbyteczne KOMPETENCJE
            var surplusKwal =  poczKwal2.Except(poczKwal1);
            var poczKwalDelete = context.PoczatkoweWydzialy.Where(p => surplusKwal.Contains(p.WydzialID) && p.Typ == cKwalifikacja);
            context.PoczatkoweWydzialy.RemoveRange(poczKwalDelete);
            await context.SaveChangesAsync();

            #endregion

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
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


        private bool ProfileExists(string id)
        {
            return userManager.Users.Any(e => e.Id == id);
        }





        [HttpGet]
        [Authorize]
        //GET : /api/UserProfile
        public async Task<ActionResult<UserModelProfil>> GetUserProfile()
        {
            string Id = User.Claims.First(c => c.Type == "UserID").Value;
            return await Profile(Id);
        }

        [HttpGet("{id}")]
        [Authorize]
        //GET : /api/UserProfile
        public async Task<ActionResult<UserModelProfil>> GetUserProfile(string id)
        {
            return await Profile(id);
        }



        private async Task<ActionResult<UserModelProfil>> Profile(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var roles = await userManager.GetRolesAsync(user);
            foreach (var r in roles)
            {
                Console.WriteLine(r.ToString());
                logger.LogWarning(r);
            };
            var poczWydz = context.PoczatkoweWydzialy.Where(w => w.UserID == user.Id && w.Typ ==cWydzial)
                .AsEnumerable().Select(w => w.WydzialID).ToArray();
            var poczKwal = context.PoczatkoweWydzialy.Where(w => w.UserID == user.Id && w.Typ ==cKwalifikacja)
                .AsEnumerable().Select(w => w.WydzialID).ToArray();
            var w = context.Wydzialy.AsEnumerable().Where(w => poczWydz.Contains(w.ID)).ToArray();
            var k = context.Wydzialy.AsEnumerable().Where(w => poczKwal.Contains(w.ID)).ToArray();
            return new UserModelProfil()
            {
                Id=id,
                UserName=user.UserName,
                FullName=user.FullName,
                Email = user.Email,
                Role=roles.ToArray(),
                PoczatkoweWydzialy = w,
                PoczatkoweKwalifikacje = k
            };
        }

        [HttpPost]
        [Authorize]
        [Route("UserProfile")]
        //Post: /api.
        public async Task<ActionResult<UserModelProfil>> SetUserProfile(UserModelProfil profile)
        {
            if (string.IsNullOrEmpty(profile.Id))
            {
                return BadRequest("Brak ID użytkownika!");
            }
            var oldUser = await userManager.FindByIdAsync(profile.Id);
            await userManager.UpdateAsync(oldUser);

            var oldRoles = await userManager.GetRolesAsync(oldUser);
            //dodaj brakujące ROLE
            var missRoles = profile.Role.Except(oldRoles);
            await userManager.AddToRolesAsync(oldUser, missRoles);
            //usuń zbyteczne ROLE
            var surplusRoles = oldRoles.Except(profile.Role);
            await userManager.RemoveFromRolesAsync(oldUser, surplusRoles);

            #region dodaj - usuń WYDZIALY
            //dodaj brakujące WYDZIAŁY
            var poczWyd1 = profile.PoczatkoweWydzialy.Select(p => p.ID).ToArray();
            var poczWyd2 = context.PoczatkoweWydzialy
                .Where(p => p.UserID == oldUser.Id && p.Typ==cWydzial).Select(p => p.WydzialID).ToArray();
            var missWydzialy = poczWyd1.Except(poczWyd2);
            var poczWydzADD = missWydzialy
                .Select(p => new PoczatkoweWydzialy
                {
                    Typ=cWydzial,
                    UserID=oldUser.Id,
                    WydzialID = p
                });
            context.PoczatkoweWydzialy.AddRange(poczWydzADD);
            await context.SaveChangesAsync();

            //usun zbyteczne WYdzialy
            var surplusWydz = poczWyd2.Except(poczWyd1);
            var poczWydzDelete = surplusWydz
                .Select(p => new PoczatkoweWydzialy
                {
                    Typ=cWydzial,
                    UserID=oldUser.Id,
                    WydzialID=p
                });
            context.PoczatkoweWydzialy.RemoveRange(poczWydzDelete);
            await context.SaveChangesAsync();
            #endregion

            #region dodaj - usuń KOMPETENCJE
            //dodaj brakujące KOMPETENCJE (WYDZIALY)
            var poczKwal1 = profile.PoczatkoweKwalifikacje.Select(k => k.ID).ToArray();
            var poczKwal2 = context.PoczatkoweWydzialy
                .Where(k => k.UserID == oldUser.Id && k.Typ == cKwalifikacja).Select(k => k.WydzialID).ToArray();
            var missKwal = poczKwal1.Except(poczKwal2);
            var poczKwalAdd = missKwal
                .Select(k => new PoczatkoweWydzialy
                {
                    Typ       = cKwalifikacja,
                    UserID    = oldUser.Id,
                    WydzialID = k
                });
            context.PoczatkoweWydzialy.AddRange(poczKwalAdd);
            await context.SaveChangesAsync();
            //usuń zbyteczne KOMPETENCJE
            var surplusKwal =  poczKwal2.Except(poczKwal1);
            var poczKwalDelete = surplusKwal.Select(k => new PoczatkoweWydzialy
            {
                Typ       = cKwalifikacja,
                UserID    = oldUser.Id,
                WydzialID = k
            });
            context.PoczatkoweWydzialy.RemoveRange(poczKwalDelete);
            await context.SaveChangesAsync();

            #endregion

            return Ok(profile);
        }

        [HttpGet("UserProfiles")]
        //[Authorize]
        //GET : /api/UserProfiles
        //[Route("UserProfiles")]
        public async Task<ActionResult<UserModelProfil[]>> GetUserProfiles()
        {
            var wydzialy         = await context.Wydzialy.ToListAsync();
            var poczWydzialy     = await context.PoczatkoweWydzialy.Where(w => w.Typ == cWydzial).ToListAsync();
            var poczKwalifikacje = await context.PoczatkoweWydzialy.Where(k => k.Typ == cKwalifikacja).ToListAsync();

            var users = await userManager.Users.ToListAsync();
            var model = users.Select(u => new UserModelProfil
            {
                Id                 = u.Id,
                Email              = u.Email,
                FullName           = u.FullName,
                UserName           = u.UserName,
                Role               = userManager.GetRolesAsync(u).Result.ToArray(),
                PoczatkoweWydzialy = wydzialy
                      .Where(w =>
                        poczWydzialy.Where(p => p.UserID == u.Id).Select(q => q.WydzialID).Contains(w.ID))
                      .ToArray(),
                PoczatkoweKwalifikacje = wydzialy
                      .Where(w =>
                        poczKwalifikacje.Where(k => k.UserID == u.Id).Select(q=> q.WydzialID).Contains(w.ID))
                      .ToArray()
            });
            return Ok(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("TestRole")]
        public string GetTestRole()
        {
            return "Webmethod for admin";
        }

    }

}