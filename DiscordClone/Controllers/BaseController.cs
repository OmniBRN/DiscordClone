using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscordClone.Controllers;

public class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly ApplicationDbContext db;
    protected readonly RoleManager<IdentityRole> _roleManager;

    public BaseController(
        UserManager<ApplicationUser> userManager
        , ApplicationDbContext context
        , RoleManager<IdentityRole> roleManager
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        
        db = context;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.Categ = GetAllCategories();
        ViewBag.fisier = "/images/defaultGroup.png";
        base.OnActionExecuting(context);
        if (User.Identity.IsAuthenticated)
        {
            
          
            var userIdVIEWBAG = _userManager.GetUserId(User); 
            var usergroupsVIEWBAG =  db.UserGroups.Where(o=>userIdVIEWBAG == o.UserId).Select(o=>o.GroupId).ToList();
            var groupsVIEWBAG = db.Groups.Where(o => usergroupsVIEWBAG.Contains(o.Id.ToString()));
            var userVIEWBAG = db.Users.Find(userIdVIEWBAG);
            if(userVIEWBAG != null)
                ViewData["User"] = userVIEWBAG;
            if(usergroupsVIEWBAG != null)
                ViewData["Groups"] = groupsVIEWBAG;
        }
        else
        {
            ViewBag.Categories = GetAllCategories();
            ViewBag.fisier = "/images/defaultGroup.png";
             Redirect("/Identity/Account/Login");
        }
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