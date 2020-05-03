using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> singInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationSettings appSettings;

        public ApplicationUserController(UserManager<ApplicationUser> userManager,
                                         SignInManager<ApplicationUser> signInManager,
                                         IOptions<ApplicationSettings> appSettings,
                                         RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.singInManager = signInManager;
            this.roleManager=roleManager;
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,

            };

            var firstUser = await FirstRegisterUser();
            try
            {
                var result = await userManager.CreateAsync(applicationUser, model.Password);
                var someAdmin = await userManager.GetUsersInRoleAsync("Admin");
                if ((firstUser && result.Succeeded) || !someAdmin.Any())
                {
                    await userManager.AddToRoleAsync(applicationUser, "Admin");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message });
            }
        }
        private async Task<bool> FirstRegisterUser()
        {
            var users = userManager.Users.Any();
            string[] roles = {"Admin","Kierownik","Brygadzista" };

            foreach (var role in roles)
            {
                var roleE = roleManager.RoleExistsAsync(role);
                if (!roleE.Result)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            return !users;
        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                //rola przypisana do użytkownika
                var role = await userManager.GetRolesAsync(user);
                IdentityOptions options = new IdentityOptions();
                IList<Claim> claims = new List<Claim>
                {
                    new Claim("UserID", user.Id.ToString())
                };
                foreach (var r in role)
                {
                    claims.Add(new Claim(options.ClaimsIdentity.RoleClaimType, r));
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(8),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "Nazwa użytkownika lub hasło są niepoprawne." });
        }
    }
}