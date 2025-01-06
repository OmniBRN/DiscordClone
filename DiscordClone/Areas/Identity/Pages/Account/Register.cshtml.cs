// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DiscordClone.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace DiscordClone.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _env = env;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            public string UserName { get; set; } 
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public IFormFile ProfilePicture { get; set; }
            //public InputModel()
            //{
            //    ProfilePicture.FileName = "wwwroot/images/defaultProfile.jpg";
            //}
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            TempData["fisier"] = "/images/defaultProfile.jpg";
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

       public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            TempData["ceva"] = "text";
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

               
                if (Input.ProfilePicture != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(Input.ProfilePicture.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ProfileImage", "fie o imagine (jpg, jpeg, png) ");
                        return Redirect("/Identity/Account/Register");
                    }

                    var storagePath = Path.Combine(_env.WebRootPath, "images", Input.ProfilePicture.FileName);
                    var databaseFileName = "/images/" + Input.ProfilePicture.FileName;

                    using (var fileStream = new FileStream(storagePath, FileMode.Create))
                    {
                        await Input.ProfilePicture.CopyToAsync(fileStream);
                    }

                    user.ProfilePicture = databaseFileName;
                }
                else
                {
                    user.ProfilePicture = "/images/defaultProfile.jpg";
                }

                

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Assign the user to the "User" role
                    await _userManager.AddToRoleAsync(user, "User");

                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    // Sign the user in directly without email verification
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        
        
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            string nume = "";
            string folder = "";
            if (file != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png"};
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                nume = Path.ChangeExtension(file.FileName, null);
                nume = nume.Replace(" ", "");
                nume += Guid.NewGuid().ToString("N") + fileExtension;
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ChannelImage",
                        "Fișierul trebuie să fie o imagine (jpg, jpeg, png).");
                   
                }

                folder = Guid.NewGuid().ToString("N");
                
                var folderName = Path.Combine(_env.WebRootPath , "temp_" + folder);
                if(!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);
                // Cale stocare
                var storagePath = Path.Combine(folderName, nume);
                var databaseFileName = "/images/"  + nume;
                TempData["fisier"] = storagePath;
                TempData["folder"] = folder;

                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                return new JsonResult(new { filePath = databaseFileName }) { StatusCode = 200 };
                
            }       

            TempData["fisier"] = nume;
            TempData["folder"] = folder;
            
            return BadRequest(new { error = "No file was uploaded." });
            
            
            return new EmptyResult();
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
