using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Pages.Identity.Admin;

public class DashboardModel : AdminPageModel
{
    public DashboardModel(UserManager<IdentityUser> userMgr) => UserManager = userMgr;
    public UserManager<IdentityUser> UserManager { get; set; }

    public int UsersCount { get; set; } = 0;
    public int UsersUnconfirmed { get; set; } = 0;
    public int UsersLockedout { get; set; } = 0;
    public int UsersTwoFactor { get; set; } = 0;

    private readonly string[] emails = {
        "alice@example.com", "bob@example.com", "charlie@example.com"
    };

    public void OnGet()
    {
        UsersCount = UserManager.Users.Count();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Notice that I use the ToList method to force evaluation in the foreach loop.
        // This ensures I don’t cause an error by deleting objects from the sequence that I am enumerating.
        foreach (IdentityUser existingUser in UserManager.Users.ToList())
        {
            IdentityResult result = await UserManager.DeleteAsync(existingUser);
            result.Process(ModelState);
        }

        foreach (string email in emails)
        {
            IdentityUser userObject = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            IdentityResult result = await UserManager.CreateAsync(userObject);
            result.Process(ModelState);
        }
        if (ModelState.IsValid)
        {
            return RedirectToPage();
        }
        return Page();
    }
}