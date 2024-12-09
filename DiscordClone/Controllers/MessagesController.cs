using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public MessagesController(
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
        //[Authorize(Roles =("User,Moderator,Admin"))]
        public IActionResult Delete( int id )
        {
            Message mes = db.Messages.Find(id);
            var par = mes.Id;

            db.Messages.Remove(mes);
            db.SaveChanges();

            //// Aici prefer sa avem o alerta decat sa creem un element cu TempData

            return Redirect("/Groups/Index");

            /// Asta ar trebui sa se intample, dar asa face figuri Index-ul si nu stiu cum sa-l repar
            ///return Redirect("/Channels/Index/" + par);
        }
    }
}
