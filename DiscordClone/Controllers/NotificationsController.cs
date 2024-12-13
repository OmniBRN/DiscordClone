using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Controllers;

public class NotificationsController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    // GET
    public IActionResult Index()
    {
        return View();
    }

    
    [HttpPost]
    public IActionResult New(int id)
    {
        var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();
        if(group == null)
            return NotFound();
        Notification notification = new Notification();
        notification.type = "Cerere intrare in grup";
        notification.UserId = group.UserId;

        var userCurent = _userManager.GetUserId(User);
        //Facem IF pt cazul in care asta este Admin ca pur si simplu sa intre, fara sa trimita notificare;


        var numeUser = db.Users.Where( a => a.Id == userCurent ).FirstOrDefault();
        notification.content = "Utilizatorul " + numeUser + " doreste sa intre in grupul "+ group.Name;
        db.Notifications.Add(notification);
        db.SaveChanges();
            

        return Redirect("/Groups/Index");
    }
    
}