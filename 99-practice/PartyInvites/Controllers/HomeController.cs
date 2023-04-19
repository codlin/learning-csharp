using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using PartyInvites.Models;

namespace PartyInvites.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public ViewResult RsvpForm()
    {
        return View();
    }

    [HttpPost]
    public ViewResult RsvpForm(GuestResponse guestResponse)
    {
        if (ModelState.IsValid)
        {
            Respository.AddResponse(guestResponse);
            return View("Thanks", guestResponse);
        }
        return View();
    }

    [HttpGet]
    public ViewResult ListResponses()
    {
        return View(Respository.Responses.Where(r => r.WillAttend == true));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
