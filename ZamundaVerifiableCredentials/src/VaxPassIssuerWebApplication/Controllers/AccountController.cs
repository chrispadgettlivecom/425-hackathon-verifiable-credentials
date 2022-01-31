using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VaxPassIssuerWebApplication.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Registration()
        {
            return View();
        }
    }
}
