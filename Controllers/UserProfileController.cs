using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        //GET : /api/UserProfile
        public async Task<Object> GetUserProfile() {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await userManager.FindByIdAsync(userId);
            var roles = await userManager.GetRolesAsync(user);
            return new
            {
                 fullName = user.FullName,
                 email = user.Email,
                 userName = user.UserName,
                 role = roles.FirstOrDefault()
            };
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        [Route("TestRole")]
        public string GetTestRole()
        {
            return "Webmethod for admin";
        }
    }
}