﻿using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers
{
    [Authorize (Roles = "Admin")]
    public class UsersController : BaseController
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            ) : base(userManager, context, roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Index()
        {

            ViewBag.CurrentUser = _userManager.GetUserId(User);
            
            var users = from user in db.Users
                        orderby user.UserName
                        select user;

            ViewBag.UsersList = users;

            if (TempData.ContainsKey("alerta"))
            {
                ViewBag.Alerta = TempData["alerta"].ToString();
                
            }

            return View();
        }
        public async Task<ActionResult> Show(string id)
        {
           
            
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            ViewBag.UserCurent = await _userManager.GetUserAsync(User);

            return View(user);
        }


        public async Task<ActionResult> Edit(string id)
        {
            
            ApplicationUser user = db.Users.Find(id);

            ViewBag.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            ViewBag.UserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
          

            
            ApplicationUser user = db.Users.Find(id);

            ViewBag.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
               /* user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.ProfilePicture = newData.ProfilePicture;
*/
                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
/*
                var admin = "2c5e174e-3b0e-446f-86af-483d56fd7210";
                var nr_admin = db.UserRoles.Where(ur => ur.RoleId == admin).ToList();
                var user_ = db.UserRoles.Where(ur => ur.UserId == id).FirstOrDefault();

                if (nr_admin.Count == 1 && user_.RoleId == admin)
                {
                    TempData["alerta"] = "Aplicatia noastra are nevoie de ( un ) admin";
                    return RedirectToAction("Index");
                }*/
                
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                TempData["alerta"] = "Ai schimbat rolul user-ului";

                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users.Include("Notifications").Where( u => u.Id == id).FirstOrDefault();
            if(user != null)
            {
                var channels = db.Channels.Where(c => c.UserId == user.Id).ToList();
                var groups = db.Groups.Where(c => c.UserId == user.Id).ToList();
                var userGroups = db.UserGroups.Where(c => c.UserId == user.Id).ToList();

                
                foreach (var userGroup in userGroups)
                {
                    var message = db.Messages.Where(o=> o.GroupId == userGroup.GroupId && o.UserId == userGroup.UserId).ToList();
                    foreach (var mes in message)
                    {
                        mes.UserId = "2ff8c808-69c7-4a2e-8271-d45ebad878df";
                    }
                   // db.Messages.RemoveRange(message);
                   // db.UserGroups.Remove(userGroup);
                   userGroup.UserId = "2ff8c808-69c7-4a2e-8271-d45ebad878df";
                   userGroup.Culoare = "gray";
                }
                
                if(channels != null)
                {
                    foreach (var channel in channels)
                    {
                        db.Channels.Remove(channel);
                    }
                }

                if (groups != null)
                {
                    foreach (var group in groups)
                    {
                        db.Groups.Remove(group);
                    }
                }
                
                db.Users.Remove(user);
                TempData["alerta"] = "Ai sters user-ul";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}
