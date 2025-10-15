using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using csharp_web_exam.Models;
using csharp_web_exam.Services;
using log4net;

namespace csharp_web_exam.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AccountController));
        private readonly ApiClient _apiClient;

        public AccountController()
        {
            _apiClient = new ApiClient();
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            
            // Si ya está autenticado, redirigir
            if (IsAuthenticated())
            {
                return RedirectToLocal(returnUrl);
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _log.Info($"Login attempt for user: {model.Username}");

                // Llamar a la API para autenticar
                var loginResponse = await _apiClient.LoginAsync(model.Username, model.Password);

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Guardar el token en una cookie
                    var cookie = new HttpCookie("AuthToken", loginResponse.Token)
                    {
                        HttpOnly = true,
                        Secure = Request.IsSecureConnection,
                        Expires = model.RememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddHours(1)
                    };
                    Response.Cookies.Add(cookie);

                    // Guardar el username en sesión
                    Session["Username"] = loginResponse.Username;
                    Session["TokenExpires"] = loginResponse.ExpiresAt;

                    _log.Info($"User {model.Username} logged in successfully");
                    TempData["Success"] = "Login successful!";

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    _log.Warn($"Login failed for user: {model.Username}");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Error during login for user: {model.Username}", ex);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
            }

            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // Eliminar la cookie de autenticación
            if (Request.Cookies["AuthToken"] != null)
            {
                var cookie = new HttpCookie("AuthToken")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Limpiar la sesión
            Session.Clear();
            Session.Abandon();

            _log.Info("User logged out");
            TempData["Success"] = "You have been logged out successfully.";

            return RedirectToAction("Login");
        }

        // Helper: Verificar si está autenticado
        private bool IsAuthenticated()
        {
            var token = Request.Cookies["AuthToken"]?.Value;
            return !string.IsNullOrEmpty(token) && Session["Username"] != null;
        }

        // Helper: Redirección segura
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Products");
        }
    }
}
