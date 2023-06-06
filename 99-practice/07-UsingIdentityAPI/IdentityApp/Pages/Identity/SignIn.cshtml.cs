using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

#nullable disable
namespace IdentityApp.Pages.Identity;

[AllowAnonymous]
public class SignInModel : UserPageModel
{
    public SignInModel(SignInManager<IdentityUser> signMgr) => SignInManager = signMgr;
    public SignInManager<IdentityUser> SignInManager { get; set; }

    [Required]
    [EmailAddress]
    [BindProperty]
    public string Email { get; set; }

    [Required]
    [BindProperty]
    public string Password { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            // persist 参数指定身份验证 cookie 是否在浏览器关闭后仍然存在。
            SignInResult result = await SignInManager.PasswordSignInAsync(Email, Password, true, true);
            if (result.Succeeded)
            {
                return Redirect(ReturnUrl ?? "/");
            }
            else if (result.IsLockedOut)
            {
                TempData["message"] = "Account Locked";
            }
            else if (result.IsNotAllowed)
            {
                TempData["message"] = "Sign In Not Allowed";
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
            }
            else
            {
                TempData["message"] = "Sign In Failed";
            }
        }
        return Page();
    }
}
