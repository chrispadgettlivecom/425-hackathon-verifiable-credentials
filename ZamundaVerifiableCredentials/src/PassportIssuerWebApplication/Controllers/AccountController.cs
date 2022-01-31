using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PassportIssuerWebApplication.Controllers
{
    public class AccountController : Controller
    {
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public IActionResult Registration()
        {
            return View();
        }
    }
}
