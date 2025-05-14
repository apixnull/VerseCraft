using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerseCraft.Data;
using VerseCraft.Models;
using VerseCraft.ViewModels;
using VerseCraft.Services;


namespace VerseCraft.Controllers
{
    public class AuthController : Controller
    {
        private readonly VerseCraftDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(VerseCraftDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        /********************************************** REGISTER **********************************************/

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsVerified = false,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["SUCCESS"] = "Account created! Check your email for the OTP.";


            await GenerateAndSendOtpAsync(user.Id, user.Email);

            return RedirectToAction("VerifyOtp", new { userId = user.Id });
        }

        /********************************************** LOGIN **********************************************/

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
           

            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.Password), "Invalid email or password.");
                return View(model);
            }

            if (!user.IsVerified)
            {
                TempData["ERROR"] = "Your account is not verified! Check your email for the OTP.";
                return RedirectToAction("VerifyOtp", new { userId = user.Id });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

            TempData["SUCCESS"] = "Logged in successfully.";

            // 🔁 Role-based redirection
            if (user.IsAdmin)
            {
                return RedirectToAction("Main", "Dashboard", new { area = "admin" });
            }

            return RedirectToAction("DisplayCollections", "MyCollection");
        }

        /********************************************** LOGOUT **********************************************/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SUCCESS"] = "Logged out successfully.";

            return RedirectToAction("Index", "Home");
        }

        /********************************************** VERIFY OTP **********************************************/

        [HttpGet]
        public async Task<IActionResult> VerifyOtp(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsVerified)
            {
                return RedirectToAction("Login");
            }

            var existingOtp = await _context.OTPTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    t.Type == OTPType.AccountVerification &&
                    t.Expiry > DateTime.UtcNow);

            if (existingOtp == null)
                await GenerateAndSendOtpAsync(userId, user.Email);

            return View(new VerifyOtpViewModel { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var otpToken = await _context.OTPTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == model.UserId &&
                    t.Type == OTPType.AccountVerification &&
                    t.Token == model.Otp);

            if (otpToken == null || otpToken.Expiry < DateTime.UtcNow)
            {
                TempData["ERROR"] = "Invalid or expired OTP.";
                return View(model);
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                TempData["ERROR"] = "User not found.";
                ModelState.AddModelError(nameof(model.Otp), model.ErrorMessage);
                return View(model);
            }

            user.IsVerified = true;
            _context.OTPTokens.Remove(otpToken);
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Your account has been verified.";
            return RedirectToAction("Login");
        }

        /********************************************** FORGET PASSWORD **********************************************/
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.PasswordHash == null)
            {
                ModelState.AddModelError(nameof(model.Email), "This email is not registered.");
                return View(model);
            }

            var otp = new OTPToken
            {
                UserId = user.Id,
                Token = new Random().Next(100000, 999999).ToString(),
                Type = OTPType.PasswordReset,
                Expiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.OTPTokens.Add(otp);
            await _context.SaveChangesAsync();

            var resetLink = Url.Action("ResetPassword", "Auth", new { userId = user.Id, token = otp.Token }, Request.Scheme);

            try
            {
                await _emailService.SendPasswordResetAsync(user.Email, resetLink!);
                model.Sent = true;
                TempData["INFO"] = "Password reset link sent to your email.";
                return View(model);
            }
            catch
            {
                ModelState.AddModelError(nameof(model.Email), "Failed to send the reset link. Please try again.");
                return View(model);
            }

        }

        /********************************************** RESET PASSWORD **********************************************/
        [HttpGet]
        public async Task<IActionResult> ResetPassword(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword");
            }

            // Check if the OTP token exists and is valid
            var otpToken = await _context.OTPTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token && t.Type == OTPType.PasswordReset && t.Expiry > DateTime.UtcNow);

            if (otpToken == null)
            {
                TempData["ERROR"] = "Invalid or expired password reset token.";
                return RedirectToAction("ForgotPassword");
            }

   
            // Show the ResetPassword view
            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var otpToken = await _context.OTPTokens
                .FirstOrDefaultAsync(t => t.UserId == model.UserId && t.Token == model.Token && t.Type == OTPType.PasswordReset && t.Expiry > DateTime.UtcNow);

            if (otpToken == null)
            {
                TempData["ERROR"] = "Reset link expired or invalid.";
                return RedirectToAction("ForgotPassword");
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("ForgotPassword");
            }

            // Reset the password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.OTPTokens.Remove(otpToken);  // Remove the OTP token after use
            await _context.SaveChangesAsync();

            TempData["SUCCESS"] = "Password reset successfully.";
            return RedirectToAction("Login");
        }

        /********************************************** RESEND OPT **********************************************/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsVerified)
                return RedirectToAction("Login");

            // Remove any existing account-verification OTPs
            var oldTokens = await _context.OTPTokens
                .Where(t => t.UserId == userId && t.Type == OTPType.AccountVerification)
                .ToListAsync();
            _context.OTPTokens.RemoveRange(oldTokens);
            await _context.SaveChangesAsync();

            // Generate and email a fresh OTP
            await GenerateAndSendOtpAsync(userId, user.Email);

            TempData["INFO"] = "A new OTP has been sent to your email.";
            return RedirectToAction("VerifyOtp", new { userId });
        }


        /********************************************** 404 NOT FOUND PAGE **********************************************/
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }

        /********************************************** HELPER **********************************************/

        private async Task GenerateAndSendOtpAsync(int userId, string email)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otpToken = new OTPToken
            {
                UserId = userId,
                Token = otpCode,
                Type = OTPType.AccountVerification,
                Expiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.OTPTokens.Add(otpToken);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpAsync(email, otpCode);
        }
    }
}
