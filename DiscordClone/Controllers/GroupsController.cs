using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers
{
    // Authorize-urile le-am comentat ca sa fie mai usor de testat functionalitatea
    // Le decomentam la testare


    // [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public GroupsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        public IActionResult Show(int id)
        {
            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
            if (group == null)
            {
                TempData["message"] = "Nu exista grupul pe care incerci sa-l accesezi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Groups", "Index");
            }

            return View(group);
        }


        // Se afiseaza lista tuturor grupurilor in care nu te afli + categoria lor
        // Acum se afiseaza toate grupurile, modific filtrarea mai incolo
        //[Authorize(Roles = "User, Moderator, Admin")]
        public IActionResult Index()
        {
            var groups = db.Groups;
            ViewBag.Groups = groups;
            
            if( TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"].ToString();
            }

            ViewBag.Categories = GetAllCategories();

            return View();
        }

        // Se afiseaza formularul in care vei crea un grup 
        //[Authorize(Roles = "User, Moderator, Admin")]
        public IActionResult New()
        {
            Group group = new Group();

            group.Categ = ViewBag.Categories ?? GetAllCategories();

            return View(group);
        }

        //[Authorize(Roles ="User, Moderator, Admin")]
        [HttpPost]
        public async Task<IActionResult> New( Group group, IFormFile ImageRPath)
        {
            if (ImageRPath == null)
            {
                var a = 1;
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                var fileExtension = Path.GetExtension(ImageRPath.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ArticleImage", "Fișierul trebuie să fie o imagine (jpg, jpeg, png, gif) sau un video (mp4,  mov).");
                    return View(group);
                }

                // Cale stocare
                var storagePath = Path.Combine(_env.WebRootPath, "images", ImageRPath.FileName);
                var databaseFileName = "/images/" + ImageRPath.FileName;

                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await ImageRPath.CopyToAsync(fileStream);
                }

                ModelState.Remove(nameof(group.ImageRPath));
                group.ImageRPath = databaseFileName;

            }
            if ( ModelState.IsValid )
            {
                group.UserId = _userManager.GetUserId(User);
                group.Date = DateTime.Now;
                db.Groups.Add(group);
                db.SaveChanges();
                TempData["message"] = "S-a creat un grup";
                TempData["messageType"] = "alert-succes";
                return RedirectToAction("Index", "Groups");
            }
            else
            {
                group.Categ = GetAllCategories();
                return View(group);
            }
        }

        public IActionResult Edit(int id)
        {
            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
            if (group == null)
            {
                TempData["message"] = "Nu exista grupul pe care incerci sa-l editezi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
                
            }
            group.Categ = GetAllCategories();
            return View(group);
            
        }

        [HttpPost]
        public IActionResult Edit(Group editedGroup)
        {
            var group = db.Groups.Where(o => o.Id == editedGroup.Id).FirstOrDefault();
            
            if (group!=null && ModelState.IsValid)
            {
                group.Name = editedGroup.Name;
                group.Description = editedGroup.Description;
                group.CategoryId = editedGroup.CategoryId;
                //Aici putem sa alegem daca vrem sa stergem poza veche a grupului de pe server sau nu
                group.ImageRPath = editedGroup.ImageRPath;
                TempData["message"] = "Grupul a fost modificat cu succes";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                // trebuie sa fac sa redirectionez in grupul pe care l-am modificat?
                return RedirectToAction("Index", "Groups");
            }
            else if (group == null)
            {
                TempData["message"] = "Nu exista grupul pe care incerci sa-l editezi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Groups");
                
            }
            else
            {
                editedGroup.Categ = GetAllCategories();
                return View(editedGroup);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {   
            
            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
            if (group == null)
            {
                TempData["message"] = "Grupul pe care incerci sa-l stergi nu exista";
                TempData["messageType"] = "alert-danger";
            }
            db.Groups.Remove(group);
            db.SaveChanges();
            TempData["message"] = "Grupul a fost sters cu succes";
            TempData["messageType"] = "alert-succes";
            return RedirectToAction("Index", "Groups");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();
            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.Id.ToString();
                listItem.Text = category.Name;

                selectList.Add(listItem);
            }
            return selectList;
        }
    }
}
