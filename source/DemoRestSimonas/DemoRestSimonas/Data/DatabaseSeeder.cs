using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoRestSimonas.Auth.Model;
using DemoRestSimonas.Data.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace DemoRestSimonas.Data
{
    public class DatabaseSeeder
    {
        private readonly UserManager<DemoRestUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(UserManager<DemoRestUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            foreach (var role in DemoRestUserRoles.All)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var newAdminUser = new DemoRestUser
            {
                UserName = "admin",
                Email = "admin@admin.com"
            };

            var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
            if (existingAdminUser == null)
            {
                var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "VerySafePassword1!");
                if (createAdminUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newAdminUser, DemoRestUserRoles.All);
                }
            }
        }
    }
}
