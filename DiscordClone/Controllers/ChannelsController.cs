using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Controllers
{
    [Authorize]
    public class ChannelsController : BaseController
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
        ) : base(userManager, context, roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        
        [HttpPost]
        [Authorize(Roles ="User, Moderator, Admin")]
        public async Task<IActionResult> ShowAsync([FromForm] Message message, IFormFile FileRPath )
        {
            
         
            
            if (FileRPath != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".mkv",".mp3"};
                var fileExtension = Path.GetExtension(FileRPath.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ChannelImage", "Fișierul trebuie să fie o imagine (jpg, jpeg, png, gif).");
                    return Redirect("/Channels/Index/" + message.MessageChannelId );
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
                if (message.Content == null)
                {
                    ModelState.Remove(nameof(message.Content));
                    message.Content = "<<empty>>";
                }
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
                // se fac chestii
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Channels/Index/" + message.MessageChannelId);
            }
            else
            {
                Channel channel = db.Channels.Include("Messages").Where(c => c.Id.ToString() == message.MessageChannelId).First();
                return Redirect($"/Channels/Index/{channel.Id}");
            }
        }
        public IActionResult Index( int id)
        {
         

            
            //// Sa incerc sa dau ToList in loc de ".First"
            var channel = db.Channels.Include(c => c.Messages).FirstOrDefault(c => c.Id == id);

            var members2 = db.UserGroups.Include("User").Where(o => o.GroupId == channel.GroupId.ToString()).Select(o=> new {o.UserId, o.Culoare, o.User.UserName, o.User.ProfilePicture, o.Role}).ToList();
            // var members1 = db.Users.Where(o => members2.Contains(o.Id));
            ViewBag.Members = members2;

            if (!members2.Any( m => m.UserId == _userManager.GetUserId(User) ))
            {
                return Redirect($"/Groups/Index");
            }
            
            // channel.Messages = db.Messages.Include("User").Where(m => m.MessageChannelId == id.ToString()).ToList();
            var mess = db.Messages
                .Where(m => m.MessageChannelId == id.ToString())
                .Join(db.Users,
                    m => m.UserId,
                    u => u.Id,
                    (m, u) => new { Message = m, User = u })
                .Join(db.UserGroups,
                    combined => new { combined.User.Id, GroupId = channel.GroupId.ToString() },
                    ug => new { Id = ug.UserId, ug.GroupId },
                    (combined, ug) => new
                    {
                        combined.Message.Id,
                        combined.Message.Content,
                        combined.Message.TimeStamp,
                        UserName = combined.User.UserName,
                        combined.User.ProfilePicture,
                        combined.Message.FileRPath,
                        Color = ug.Culoare, 
                        Rol = ug.Role
                    })
                .ToList();
            getId();
            
            var role = db.UserGroups
                .Where(o => _userManager.GetUserId(User) == o.UserId && channel.GroupId.ToString() == o.GroupId)
                .Select(o => o.Role).FirstOrDefault();
            ViewBag.OwnerId = db.Groups.Where(o=> o.Id == channel.GroupId).Select(o=>o.UserId).First();
            ViewBag.Messages = mess;
            ViewBag.UserCurrentRole = role;
            return View(channel);
        }

        [HttpPost]
        public IActionResult Promote(string UserId, string GroupId, int ChannelId)
        {
            
            var userGroup = db.UserGroups.Where(o=> o.GroupId == GroupId && o.UserId == UserId).FirstOrDefault();
            Console.WriteLine(GroupId);
            Console.WriteLine(UserId);
            Console.WriteLine(ChannelId);   
            userGroup.Role = "Moderator";
            userGroup.Culoare = "#F9C74F";
            db.SaveChanges();
            return Redirect("/Channels/Index/" + ChannelId);
        }
        
        [HttpPost]
        public IActionResult Demote(string UserId, int GroupId, int ChannelId)
        {
            
            var userGroup = db.UserGroups.Where(o=> o.GroupId == GroupId.ToString() && o.UserId == UserId).FirstOrDefault();
            userGroup.Role = "User";
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
            userGroup.Culoare = colors[randomNumber];
            db.SaveChanges();
            return Redirect("/Channels/Index/" + ChannelId);
        }


        [NonAction]
        private void getId()
        {
            ViewBag.UserCurrent = _userManager.GetUserId(User);
          
        }
    }
}
