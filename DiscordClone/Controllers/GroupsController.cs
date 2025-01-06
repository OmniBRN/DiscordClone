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


    //[Authorize]


    public class GroupsController : BaseController
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
        ) : base(userManager, context, roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        //[Authorize(Roles ="User,Moderator,Admin")]



        public IActionResult Show(int id)
        {



            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
            if (group == null)
            {
                TempData["message"] = "Nu exista grupul pe care incerci sa-l accesezi";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Groups", "Index");
            }

            var t = db.Users.Where(o => o.Id == group.UserId).FirstOrDefault().UserName;
            if (t != null)
                ViewBag.Owner = t;



            return View(group);
        }


        // Se afiseaza lista tuturor grupurilor in care nu te afli + categoria lor
        // Acum se afiseaza toate grupurile, modific filtrarea mai incolo
        //[Authorize(Roles = "User, Moderator, Admin")]
        public IActionResult Index()
        {

            ViewBag.ToateGrupurile = db.Groups;
            
            
            

            var userId = _userManager.GetUserId(User);
            var allGroupsImin = db.UserGroups.Where(o => userId == o.UserId && o.Culoare != "gray").Select(o => o.GroupId).ToList();
            var groups = db.Groups.Where(o => !allGroupsImin.Contains(o.Id.ToString())).ToList();
            var groupsIds = groups.Select(o => o.Id).ToList();
            
            
            var folderName = _env.WebRootPath + "/temp_" + userId;
            if(Directory.Exists(folderName))
                Directory.Delete(folderName, true);

            // var groups = db.Groups.Where(o => allGroupsNotIn.Contains(o.Id.ToString()));



            if (TempData.ContainsKey("alerta"))
            {
                ViewBag.Alerta = TempData["alerta"].ToString();
                // ViewBag.Alert = TempData["messageType"].ToString();
            }

            ViewBag.Categories = GetAllCategories();


            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                List<int> groupsId = db.Groups.Where(at => at.Name.Contains(search)).Select(a => a.Id).ToList();
                var groups2 = db.Groups.Where(o => groupsId.Contains(o.Id) && groupsIds.Contains(o.Id))
                    .OrderByDescending(a => a.Name).ToList();
                groups = groups2;
            }

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Groups/Index/?search=" + search;

            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Groups/Index";
            }

            ViewBag.SearchString = search;

            ViewBag.Groups2 = groups;
            return View();
        }

        // Se afiseaza formularul in care vei crea un grup 
        [Authorize(Roles = "User, Moderator, Admin")]
        public IActionResult New()
        {


            Group group = new Group();

            group.Categ = GetAllCategories();
            ViewBag.fisier = "/images/defaultGroup.png";
            return View(group);
        }

        [Authorize(Roles = "User, Moderator, Admin")]
        [HttpPost]
        public async Task<IActionResult> New(Group group, IFormFile ImageRPath)
        {

            ViewBag.fisier = "/images/defaultGroup.png";
            group.Categ= GetAllCategories();
            ModelState.Remove(nameof(group.ImageRPath));

            if (TempData.ContainsKey("fisier") && TempData["fisier"] != null)
            {
                var numeFisier = TempData["fisier"].ToString();
                TempData["fisier"] = null;
                var numeFolder = Path.Combine(_env.WebRootPath, "temp_" + _userManager.GetUserId(User));
                var file = new FileInfo(Path.Combine(numeFolder, numeFisier));
                file.MoveTo(Path.Combine(_env.WebRootPath, "images", numeFisier));

                // Cale stocare
                
                var databaseFileName = "/images/" + numeFisier;
                
                ModelState.Remove(nameof(group.ImageRPath));
                group.ImageRPath = databaseFileName;

            }
            else
            {
                group.ImageRPath = "/images/defaultGroup.png";
            }


            if (ModelState.IsValid)
            {


                Channel channel = new Channel();
                channel.Name = "Canalul: " + group.Name;
                channel.Description = "Acesta este canalul default";
                channel.UserId = _userManager.GetUserId(User);

                group.UserId = _userManager.GetUserId(User);
                group.Date = DateTime.Now;

                db.Groups.Add(group);
                db.SaveChanges();

                var userGroup = new UserGroup();
                userGroup.GroupId = group.Id.ToString();
                userGroup.UserId = _userManager.GetUserId(User);
                userGroup.Role = "Moderator";
                userGroup.Culoare = "#F9C74F";
                db.UserGroups.Add(userGroup);
                db.SaveChanges();

                channel.GroupId = group.Id;
                db.Channels.Add(channel);
                db.SaveChanges();

                group.GroupChannelId = channel.Id;
                db.SaveChanges();

                TempData["alerta"] = "S-a creat un grup";
                
                return RedirectToAction("Index", "Groups");
            }
            else
            {
                group.Categ = GetAllCategories();
                return View(group);
            }
        }
        
        [HttpPost]
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
                // Cale stocare
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
        
        
        [Authorize]
        public IActionResult Edit(int id)
        {


            var userCurrentId = _userManager.GetUserId(User);
            var currentGrupId = db.Groups.Find(id).Id;
            var currentUserRole = db.UserGroups.Where(o=>o.GroupId == currentGrupId.ToString() && o.UserId == userCurrentId).First().Role;
            if(currentUserRole != "Moderator")
                return RedirectToAction("Index", "Groups");

           
            

            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
            if (group == null)
            {
                TempData["alerta"] = "Nu exista grupul pe care incerci sa-l editezi";
                
                return RedirectToAction("Index", "Groups");

            }

            ViewBag.fisier = group.ImageRPath;
            group.Categ = GetAllCategories();
            return View(group);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Group editedGroup, IFormFile ImageRPath)
        {

            var userCurrentId = _userManager.GetUserId(User);
            var currentGrupId = db.Groups.Find(editedGroup.Id).Id;
            var currentUserRole = db.UserGroups.Where(o=>o.GroupId == currentGrupId.ToString() && o.UserId == userCurrentId).First().Role;
            if(currentUserRole != "Moderator")
                return RedirectToAction("Index", "Groups");
            
            
            var group = db.Groups.Where(o => o.Id == editedGroup.Id).FirstOrDefault();
            if (TempData.ContainsKey("fisier") && TempData["fisier"] != null)
            {
                var numeFisier = TempData["fisier"].ToString();
                TempData["fisier"] = null;
                var numeFolder = Path.Combine(_env.WebRootPath, "temp_" + _userManager.GetUserId(User));
                var file = new FileInfo(Path.Combine(numeFolder, numeFisier));
                file.MoveTo(Path.Combine(_env.WebRootPath, "images", numeFisier));

                // Cale stocare
                
                var databaseFileName = "/images/" + numeFisier;
                
                ModelState.Remove(nameof(group.ImageRPath));
                group.ImageRPath = databaseFileName;

            }
            else
            {
                ModelState.Remove(nameof(group.ImageRPath));
            }
            
            if (group != null && ModelState.IsValid)
            {
                group.Name = editedGroup.Name;
                group.Description = editedGroup.Description;
                group.CategoryId = editedGroup.CategoryId;
                
                TempData["alerta"] = "Grupul a fost modificat cu succes";
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
            var userCurrentId = _userManager.GetUserId(User);
            var currentGrupId = db.Groups.Find(id).Id;
            var currentUserRole = "";
            if (User.IsInRole("Admin"))
            {
                currentUserRole = "Moderator";
            }
            else
            {
                currentUserRole = db.UserGroups.Where(o=>o.GroupId == currentGrupId.ToString() && o.UserId == userCurrentId).FirstOrDefault().Role;
                
            }
            if(!((currentUserRole == "Moderator"&& group.UserId == userCurrentId) || User.IsInRole("Admin")))
                return RedirectToAction("Index", "Groups");

            //// Aici am adaugat comentariu pt ca ar trebui sa stergem si canalele asociate grupului pe care-l facem

            var channels = db.Channels.Include("Messages").Where(o => o.GroupId == id).FirstOrDefault();
            if (group == null)
            {
                TempData["alerta"] = "Grupul pe care incerci sa-l stergi nu exista";
                
            }
            
            if (group.ImageRPath != null && group.ImageRPath != "/images/defaultGroup.png")
            {
                string filePath = Path.Combine(_env.WebRootPath, group.ImageRPath.TrimStart('/').Replace("/", "\\"));
                System.IO.File.Delete(filePath);
            }

            db.Groups.Remove(group);
            //Am facut un check sa vad daca este null channels ca altfel da eroare
            if (channels != null)
            {
                var mesaje = db.Messages.Where(m => m.MessageChannelId == id.ToString()).ToList();
                
                if (mesaje.Count > 0)
                {
                    foreach (var mesaj in mesaje)
                    {
                        if (mesaj.FileRPath != null)
                        {
                            string filePath = Path.Combine(_env.WebRootPath, mesaj.FileRPath.TrimStart('/').Replace("/", "\\"));
                            System.IO.File.Delete(filePath);
                            mesaj.FileRPath = null;
                        }
                        db.Messages.Remove(mesaj);
                    }
                }

                
                
                db.Channels.Remove(channels);
            }
            db.SaveChanges();
            TempData["alerta"] = "Grupul a fost sters cu succes";
            
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
