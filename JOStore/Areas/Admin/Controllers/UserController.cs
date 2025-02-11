using JOStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;

namespace JOStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UserController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(AppDbContext appDbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            string roleId = _appDbContext.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            RoleManagmentVM roleManagmentVM = new RoleManagmentVM() 
            {
                AppUser = _appDbContext.AppUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _appDbContext.Roles.Select(i => new SelectListItem{
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _appDbContext.Companies.Select(i => new SelectListItem{
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

            }; 
            roleManagmentVM.AppUser.Role = _appDbContext.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            return View(roleManagmentVM);
        }
        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var userRole = _appDbContext.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.AppUser.Id);
            string roleId = userRole?.RoleId;
            string oldRole = _appDbContext.Roles.FirstOrDefault(u => u.Id == roleId)?.Name ?? "Unknown";

            if (roleManagmentVM.AppUser.Role != oldRole)
            {
                AppUser appUser = _appDbContext.AppUsers.FirstOrDefault(u => u.Id == roleManagmentVM.AppUser.Id);

                if (roleManagmentVM.AppUser.Role == SD.Role_Company)
                {
                    appUser.CompanyId = roleManagmentVM.AppUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    appUser.CompanyId = null;
                }

                _appDbContext.SaveChanges();

                // Use await instead of blocking the thread
                await _userManager.RemoveFromRoleAsync(appUser, oldRole);
                await _userManager.AddToRoleAsync(appUser, roleManagmentVM.AppUser.Role);
            }
            return RedirectToAction(nameof(Index));
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _appDbContext.AppUsers
                .Include(u => u.Company)
                .Select(user => new
                {
                    user.Id,
                    Name = user.Name ?? "N/A",
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber ?? "N/A",
                    CompanyName = user.Company != null ? user.Company.Name : "N/A",
                    Role = _appDbContext.Roles
                            .Where(r => r.Id == _appDbContext.UserRoles
                            .FirstOrDefault(ur => ur.UserId == user.Id).RoleId)
                            .Select(r => r.Name)
                            .FirstOrDefault() ?? "Unknown",
                    LockoutEnd = user.LockoutEnd
                }).ToList();

            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Now); // Unlock user
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(1000)); // Lock user
            }

            return Json(new { success = true, message = $"User {(user.LockoutEnd > DateTime.Now ? "Locked" : "Unlocked")} Successfully" });
        }


    }
}

#endregion
