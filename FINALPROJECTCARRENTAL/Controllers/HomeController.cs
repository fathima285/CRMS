using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarRentalSystem.Models;
using CarRentalSystem.Services;

namespace CarRentalSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        // Check if user is logged in and redirect to appropriate dashboard
        var role = HttpContext.Session.GetString("Role");
        if (!string.IsNullOrEmpty(role))
        {
            if (role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Customer");
            }
        }

        // If not logged in, show guest landing page
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(string firstName, string lastName, string email, string phone, string subject, string message)
    {
        var fullName = $"{firstName} {lastName}".Trim();
        var emailSubject = string.IsNullOrWhiteSpace(subject) ? "New Contact Message" : $"Contact: {subject}";
        var body = $@"<h3>New Contact Message</h3>
                      <p><strong>Name:</strong> {fullName}</p>
                      <p><strong>Email:</strong> {email}</p>
                      <p><strong>Phone:</strong> {phone}</p>
                      <p><strong>Subject:</strong> {subject}</p>
                      <p><strong>Message:</strong><br/>{System.Net.WebUtility.HtmlEncode(message).Replace("\n", "<br/>")}</p>";

        await _emailService.SendDefaultAsync(emailSubject, body);
        TempData["SuccessMessage"] = "Thanks! Your message has been sent.";
        return RedirectToAction("Contact");
    }

    public IActionResult Settings()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
