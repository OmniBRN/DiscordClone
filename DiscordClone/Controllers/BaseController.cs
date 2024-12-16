using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
             Redirect("/Identity/Account/Login");
        }
    }
}