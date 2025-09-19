using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.Models.ViewModels;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ================== LOGIN ==================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    if (!user.IsEmailVerified)
                    {
                        ModelState.AddModelError("", "Email not verified. Please check your inbox.");
                        return View(model);
                    }

                    // Save session values
                    HttpContext.Session.SetString("UserID", user.UserID.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);

                    // Redirect by role
                    if (user.Role == "Admin")
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        // ================== REGISTER ==================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                // Hash password before saving
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var verificationCode = Guid.NewGuid().ToString("N");
                var verificationExpiresAt = DateTime.UtcNow.AddHours(2);

                var user = new User
                {
                    Username = model.Username,
                    Password = hashedPassword,   // save hashed
                    Role = "Customer",
                    Email = model.Email,
                    IsEmailVerified = false,
                    VerificationCode = verificationCode,
                    VerificationExpiresAt = verificationExpiresAt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Send verification email
                var verifyLink = Url.Action("Verify", "Account", new { code = verificationCode }, Request.Scheme);
                var subject = "Verify your email - Car Rental System";
                var body = $@"<p>Hi {System.Net.WebUtility.HtmlEncode(model.Username)},</p>
                              <p>Please verify your email by clicking the link below:</p>
                              <p><a href='{verifyLink}'>Verify Email</a></p>
                              <p>Or use this code: <strong>{verificationCode}</strong></p>
                              <p>This link/code expires at {verificationExpiresAt:u} (UTC).</p>";

                await _emailService.SendAsync(model.Email, subject, body);

                TempData["SuccessMessage"] = "Registration successful! Check your email for the verification link.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // ================== EMAIL VERIFY ==================
        [HttpGet]
        public async Task<IActionResult> Verify(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["ErrorMessage"] = "Invalid verification code.";
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationCode == code);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Verification failed. Code not found.";
                return RedirectToAction("Login");
            }

            if (user.VerificationExpiresAt.HasValue && user.VerificationExpiresAt.Value < DateTime.UtcNow)
            {
                TempData["ErrorMessage"] = "Verification code expired. Please register again.";
                return RedirectToAction("Login");
            }

            user.IsEmailVerified = true;
            user.VerificationCode = null;
            user.VerificationExpiresAt = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Email verified successfully. Please login.";
            return RedirectToAction("Login");
        }

        // ================== LOGOUT ==================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
