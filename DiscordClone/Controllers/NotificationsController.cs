using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public NotificationsController(
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(int id)
        {
            Console.WriteLine(id);

            var group = db.Groups.Where(o => o.Id == id).FirstOrDefault();

            Notification notification = new Notification();
            notification.type = "Cerere intrare in grup";
            notification.UserId = group.UserId;

            var userCurent = _userManager.GetUserId(User);
            //Facem IF pt cazul in care asta este Admin ca pur si simplu sa intre, fara sa trimita notificare;


            var numeUser = db.Users.Where(a => a.Id == userCurent).FirstOrDefault().UserName;
            notification.content = "Utilizatorul " + numeUser + " doreste sa intre in grupul " + group.Name;
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
            return Redirect("/Groups/Index");
        }

    }
}
