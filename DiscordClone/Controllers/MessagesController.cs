using System.Runtime.InteropServices.JavaScript;
using DiscordClone.Data;
using DiscordClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.Controllers
{
    [Authorize]
    public class MessagesController : BaseController
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
        ) : base(userManager, context, roleManager)
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
        public IActionResult DeleteFile(int id)
        {
            Message mes = db.Messages.Find(id);
            string filePath = Path.Combine(_env.WebRootPath, mes.FileRPath.TrimStart('/').Replace("/", "\\"));
            System.IO.File.Delete(filePath);
            mes.FileRPath = null;
            if (mes.Content == "<<empty>>")
            {
                db.Messages.Remove(mes);
            }
            db.SaveChanges();
            
            return Redirect($"/Channels/Index/{mes.MessageChannelId}");
        }

        [HttpPost]
        //[Authorize(Roles =("User,Moderator,Admin"))]
        public IActionResult Delete(int id)
        {
          
            Message mes = db.Messages.Find(id);
            
          
            var group = db.Groups.Where(o => o.Id.ToString() == mes.GroupId).FirstOrDefault();
            var userCurrentId = _userManager.GetUserId(User);
            var currentUserRole = db.UserGroups.Where(o=>o.GroupId == mes.GroupId.ToString() && o.UserId == userCurrentId).First().Role;
            if(!((currentUserRole == "Moderator" || group.UserId == userCurrentId) || User.IsInRole("Admin")))
                
                return RedirectToAction("Index", "Groups");
            
            
            var par = mes.Id;
            
            if (mes.FileRPath != null)
            {
                string filePath = Path.Combine(_env.WebRootPath, mes.FileRPath.TrimStart('/').Replace("/", "\\"));
                System.IO.File.Delete(filePath);
                mes.FileRPath = null;
                
            }

            db.Messages.Remove(mes);
            db.SaveChanges();

            //// Aici prefer sa avem o alerta decat sa creem un element cu TempData
            // RE: Nu cred ca ar trebui sa alertam  ca s-au sters mesaje ca se observa
            return Redirect($"/Channels/Index/{mes.MessageChannelId}");

            /// Asta ar trebui sa se intample, dar asa face figuri Index-ul si nu stiu cum sa-l repar
            ///return Redirect("/Channels/Index/" + par);
        }
        

        [HttpPost]
        public IActionResult Edit(Message NewMessage)
        {
            Message oldMessage = db.Messages.Find(NewMessage.Id);
            
            
            
            oldMessage.Content = NewMessage.Content;
            oldMessage.WasEdited = true;
            oldMessage.TimeStamp = DateTime.Now;
            db.SaveChanges();
            return Redirect($"/Channels/Index/{NewMessage.MessageChannelId}");
        }
        
        
    }
}
