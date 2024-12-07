using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers
{
    public class ChannelsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public ChannelsController(
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

        [HttpPost]
        //[Authorize(Roles ="User, Editor, Admin")]
        public IActionResult Show([FromForm] Message message )
        {
            message.TimeStamp = DateTime.Now;
            message.UserId = _userManager.GetUserId(User);

            if( ModelState.IsValid )
            {
                /// se fac chestii
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Groups/Index");
            }
            else
            {
                Channel channel = db.Channels.Include("Category").Include("Messages").Where(c => c.Id.ToString() == message.ChannelId).First();
                return View(channel);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
