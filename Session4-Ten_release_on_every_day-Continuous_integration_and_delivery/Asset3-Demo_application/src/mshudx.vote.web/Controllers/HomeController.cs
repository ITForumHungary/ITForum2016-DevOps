using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mshudx.Vote.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SignIn()
        {
            var nameIdentifier = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity(
                new []
                {
                    new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                    new Claim(ClaimTypes.Name, nameIdentifier),
                    
                },
                "auth.mshudx.vote");
            await HttpContext.Authentication.SignInAsync(
                        "auth.mshudx.vote", 
                        new ClaimsPrincipal(identity));
                        
            return RedirectToAction("Index");
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}
