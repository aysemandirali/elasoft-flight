using System.Security.Claims;
using FlightBooking.Services.AccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly IAuthService _authService;

        public SettingsController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            ViewBag.FullName = User.Identity?.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string newPasswordRepeat)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            ViewBag.Email = email;
            ViewBag.FullName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["PwError"] = "Yeni şifre en az 6 karakter olmalı.";
                return RedirectToAction("Index");
            }
            if (newPassword != newPasswordRepeat)
            {
                TempData["PwError"] = "Yeni şifreler birbiriyle eşleşmiyor.";
                return RedirectToAction("Index");
            }

            var ok = await _authService.ChangePasswordAsync(email, currentPassword, newPassword);
            if (!ok)
                TempData["PwError"] = "Mevcut şifre hatalı.";
            else
                TempData["PwSuccess"] = "Şifreniz başarıyla güncellendi.";

            return RedirectToAction("Index");
        }
    }
}
