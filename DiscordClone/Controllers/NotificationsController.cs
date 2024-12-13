using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DiscordClone.Controllers;

public class NotificationsController : BaseController
{
    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    // GET
    public NotificationsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment env
    ) : base(userManager, context, roleManager)
    {
        db = context;
        _userManager = userManager;
        _roleManager = roleManager;
        
    }
    public IActionResult Index()
    {
        

        
        var userId = _userManager.GetUserId(User);
        var notifications = db.Notifications.Where(o=> o.UserId == userId);
        ViewBag.Notifications = notifications;
        return View();
    }

    
    [HttpPost]
    public IActionResult New(int id)
    {
        
        
        Console.WriteLine(id);
        
        var group = db.Groups.Where(o=>o.Id == id).FirstOrDefault();
        
        Notification notification = new Notification();
        notification.type = "Cerere intrare in grup";
        notification.UserId = group.UserId;
        notification.ReferencedGroupId = group.Id.ToString();
        var userCurent = _userManager.GetUserId(User);
        notification.FromUserId = userCurent;
        
        var t = db.UserGroups.Where(o=> o.GroupId == notification.ReferencedGroupId && o.UserId == notification.FromUserId).FirstOrDefault();
        if (t != null)
        {   
            //Mai tarziu trebuie sa adaugam o alerta care sa spuna ca ala care a facut cererea este dezabilitat
            return Redirect("/Groups/Index/");
        }
        
        //Facem IF pt cazul in care asta este Admin ca pur si simplu sa intre, fara sa trimita notificare;


        var numeUser = db.Users.Where(a => a.Id == userCurent).FirstOrDefault().UserName;
        notification.content = "Utilizatorul " + numeUser + " doreste sa intre in grupul "+ group.Name;
        
        db.Notifications.Add(notification);
        db.SaveChanges();
        

        return Redirect("/Groups/Index");
       
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
       
        
        var notification = db.Notifications.Where(o => o.Id == id).FirstOrDefault();
        db.Notifications.Remove(notification);
        db.SaveChanges();
        return Redirect("/Notifications/Index");
    }

    [HttpPost]
    public IActionResult Accept(int id)
    {
      

        
        var notification = db.Notifications.Where(o => o.Id == id).FirstOrDefault();
        var usergroup = new UserGroup();
        usergroup.GroupId = notification.ReferencedGroupId;
        usergroup.UserId = notification.FromUserId;
        string[] colors = new string[]
        {
            "#F94144", // Vibrant Red
            "#F3722C", // Warm Orange
            "#F8961E", // Golden Amber
            "#F9844A", // Sunset Peach
            "#90BE6D", // Soft Olive Green
            "#43AA8B", // Teal
            "#577590", // Muted Blue
            "#277DA1", // Deep Cyan
            "#4D908E", // Slate Green
            "#577590", // Steely Blue
            "#F4A261", // Terracotta
            "#D72638", // Crimson
            "#3F88C5", // Cerulean Blue
            "#1446A0", // Royal Blue
            "#585123", // Earthy Brown
            "#A78682", // Dusty Rose
            "#5A5A66", // Charcoal Grey
            "#C9ADA7", // Pale Mauve
            "#9C6644"  // Cocoa Brown
        };
        Random random = new Random();
        int randomNumber = random.Next(colors.Length);
        usergroup.Culoare = colors[randomNumber];
        usergroup.Role = "User";
        db.UserGroups.Add(usergroup);
        db.SaveChanges();
        
        db.Notifications.Remove(notification);
        db.SaveChanges();
        
        return Redirect("/Notifications/Index");
    }
    
}