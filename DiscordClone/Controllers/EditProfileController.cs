using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers;

public class EditProfileController: BaseController
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment _env;
    
    public EditProfileController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment env,
        SignInManager<ApplicationUser> signInManager
    ) : base(userManager, context, roleManager)
    {
        db = context;

        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _env = env;
    }
    
    public async Task<ActionResult> Edit(string id)
    {
            
        ApplicationUser user = db.Users.Find(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var currentuser = _userManager.GetUserId(User);
        if (id != currentuser)
        {
            TempData["alerta"] = "Nu poti accesa contul altui utilizator";
            return Redirect("/Groups/Index/");
        }
        
        ViewBag.fisier = user.ProfilePicture;
        
        return View(user);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
    {
        ApplicationUser user = db.Users.Find(id);
        
       
        
        if (TempData.ContainsKey("fisier") && TempData["fisier"] != null)
        {
            var numeFisier = TempData["fisier"].ToString();
            TempData["fisier"] = null;
            var numeFolder = Path.Combine(_env.WebRootPath, "temp_" + _userManager.GetUserId(User));
            var file = new FileInfo(Path.Combine(numeFolder, numeFisier));
            file.MoveTo(Path.Combine(_env.WebRootPath, "images", numeFisier));

            // Cale stocare
                
            var databaseFileName = "/images/" + numeFisier;
                
            ModelState.Remove(nameof(user.ProfilePicture));
            user.ProfilePicture = databaseFileName;
            ViewBag.fisier = user.ProfilePicture;

        }
        else
        {
            ModelState.Remove(nameof(user.ProfilePicture));
        }
        
        ModelState.Remove(nameof(newRole));
        
        if (ModelState.IsValid)
        {
            
            user.UserName = newData.UserName;
            user.NormalizedUserName = newData.UserName.ToUpper();
            user.Email = newData.Email;
            if(TempData.ContainsKey("fisier") && TempData["fisier"] != null)
                user.ProfilePicture = TempData["fisier"].ToString();

            // Cautam toate rolurile din baza de date
            var roles = db.Roles.ToList();
            
            TempData["alerta"] = "Schimbarile au fost efectuate";
            Console.WriteLine("ceva");

            db.SaveChanges();

        }
        return Redirect("/Groups/Index");
    }
    
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        string nume = "";
        if (file != null)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png"};
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            nume = Path.ChangeExtension(file.FileName, null);
            nume = nume.Replace(" ", "");
            nume += Guid.NewGuid().ToString("N") + fileExtension;
                
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ChannelImage",
                    "Fișierul trebuie să fie o imagine (jpg, jpeg, png).");
                return Json(new { message = "File received has incorrect format." });
            }
            var folderName = Path.Combine(_env.WebRootPath , "temp_" + _userManager.GetUserId(User));
            if(!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            
            var storagePath = Path.Combine(folderName, nume);
            var databaseFileName = "/images/"  + nume;
            ViewBag.fisier = storagePath;

            // Salvare fișier
            using (var fileStream = new FileStream(storagePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

        }       

        TempData["fisier"] = nume;
            
        return Json(new { message = "✓" });
    }
    
    [HttpPost]
    public IActionResult Delete(string id)
    {
        var user = db.Users.Include("Notifications").Where( u => u.Id == id).FirstOrDefault();
        var admin = "2c5e174e-3b0e-446f-86af-483d56fd7210";
        var nr_admin = db.UserRoles.Where(ur => ur.RoleId == admin).ToList();
        var user_ = db.UserRoles.Where(ur => ur.UserId == id).FirstOrDefault();

        if (nr_admin.Count == 1 && user_.RoleId == admin)
        {
            TempData["alerta"] = "Aplicatia noastra are nevoie de ( un ) admin";
            return Redirect("/Groups/Index");
        }
        
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
            
            _signInManager.SignOutAsync();


            return RedirectToAction("Index", "Groups", new { area = "" });
        }
        return RedirectToAction("Index");
    }
    
    
}