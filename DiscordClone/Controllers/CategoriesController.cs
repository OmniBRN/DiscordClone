using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscordClone.Controllers;

[Authorize (Roles="Admin")]
public class CategoriesController : BaseController
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment _env;

    
    public CategoriesController(
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
    // GET
    public IActionResult Index()
    {
       

        
        var categories = from category in db.Categories
            orderby category.Name
            select category;
        ViewBag.Categories = categories;
        
        if (TempData.ContainsKey("alerta"))
        {
            ViewBag.Alerta = TempData["alerta"].ToString();
        }
        
        return View();
        
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        string nume = "";
        string folder = "";
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
                   
            }

            folder = Guid.NewGuid().ToString("N");
                
            var folderName = Path.Combine(_env.WebRootPath , "temp_" + folder);
            if(!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            // Cale stocare
            var storagePath = Path.Combine(folderName, nume);
            var databaseFileName = "/images/"  + nume;
            TempData["fisier"] = storagePath;
            TempData["folder"] = folder;

            // Salvare fișier
            using (var fileStream = new FileStream(storagePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
                
            return new JsonResult(new { filePath = databaseFileName }) { StatusCode = 200 };
                
        }       

        TempData["fisier"] = nume;
        TempData["folder"] = folder;
            
        return BadRequest(new { error = "No file was uploaded." });
            
            
        return new EmptyResult();
    }


    public ActionResult New()
    {

        
        return View();
    }

    [HttpPost]
    public ActionResult New(Category cat)
    {
        
        
        if (ModelState.IsValid)
        {
            db.Categories.Add(cat);
            db.SaveChanges();
            TempData["alerta"] = "Categoria a fost adaugata";
            return RedirectToAction("Index");
        }

        else
        {
            return View(cat);
        }
    }

    public ActionResult Edit(int id)
    {
       

        
        Category category = db.Categories.Find(id);
        return View(category);
    }

    [HttpPost]
    public ActionResult Edit(int id, Category requestCategory)
    {
        Category category = db.Categories.Find(id);

        if (ModelState.IsValid)
        {

            category.Name = requestCategory.Name;
            db.SaveChanges();
            TempData["alerta"] = "Categoria a fost modificata!";
            return RedirectToAction("Index");
        }
        else
        {
            return View(requestCategory);
        }
    }

    [HttpPost]
    public ActionResult Delete(int id)
    {
      
        
        Category category = db.Categories.Find(id);
        db.Categories.Remove(category);
        TempData["alerta"] = "Categoria a fost stearsa";
        db.SaveChanges();
        return RedirectToAction("Index");
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