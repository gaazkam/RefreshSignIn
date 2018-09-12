using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RefreshSignInNotWorking.Pages
{
    public class BanModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public BanModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public class InputModel
        {
            public string UsernameToBan { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet()
        {
            if (User.IsInRole("Banned"))
                return Unauthorized();

            return Page();
        }

        public string Message { get; set; } = null;

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.IsInRole("Banned"))
                return Unauthorized();

            if (ModelState.IsValid)
            {
                var bannedUser = await userManager.FindByNameAsync(Input.UsernameToBan);
                if (bannedUser != null)
                {
                    await userManager.AddToRoleAsync(bannedUser, "Banned");
                    await signInManager.RefreshSignInAsync(bannedUser);
                    Message = "Succesfully banned user " + Input.UsernameToBan + ".";
                }
            }
            return Page();
        }
    }
}