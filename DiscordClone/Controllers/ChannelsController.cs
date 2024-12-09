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
        public async Task<IActionResult> ShowAsync([FromForm] Message message, IFormFile FileRPath )
        {
            
            if (FileRPath != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                var fileExtension = Path.GetExtension(FileRPath.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ChannelImage", "Fișierul trebuie să fie o imagine (jpg, jpeg, png, gif).");
                    return Redirect("Channels/Index/" + message.ChannelId );
                }

                // Cale stocare
                var storagePath = Path.Combine(_env.WebRootPath, "images", FileRPath.FileName);
                var databaseFileName = "/images/" + FileRPath.FileName;

                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await FileRPath.CopyToAsync(fileStream);
                }

                ModelState.Remove(nameof(message.FileRPath));
                message.FileRPath = databaseFileName;

            }
            else
            {
                ModelState.Remove(nameof(message.FileRPath));
            }

            message.TimeStamp = DateTime.Now;
            message.UserId = _userManager.GetUserId(User);

            if( ModelState.IsValid )
            {
                /// se fac chestii
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Channels/Index/" + message.ChannelId);
            }
            else
            {
                Channel channel = db.Channels.Include("Messages").Where(c => c.Id.ToString() == message.ChannelId).First();
                return View("Index/" + channel.Id, channel);
            }
        }
        public IActionResult Index( int id)
        {
            //// Sa incerc sa dau ToList in loc de ".First"
            var channel = db.Channels.Include(c => c.Messages).FirstOrDefault(c => c.Id == id);
            channel.Messages = db.Messages.Where(m => m.ChannelId == id.ToString()).ToList();
            return View(channel);
        }
    }
}
