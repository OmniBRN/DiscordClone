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
        
        return View();
        
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
            TempData["message"] = "Categoria a fost adaugata";
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
            TempData["message"] = "Categoria a fost modificata!";
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
        TempData["message"] = "Categoria a fost stearsa";
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