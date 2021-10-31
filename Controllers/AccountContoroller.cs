using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DockerPipelineExample.Controllers
{
  public class AccountController : Controller
  {
    public IActionResult LoginWithAzure()
    {
      if(!HttpContext.User.Identity.IsAuthenticated)
      {
        return Challenge("aad");
      }
      return RedirectToAction("Index", "Home");
    }

    public IActionResult LoginWithOkta()
    {
      if(!HttpContext.User.Identity.IsAuthenticated)
      {
        return Challenge("okta");
      }
      return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    { 
      // HACK
      var oidcAuthenticationScheme = "okta";
      foreach (var claim in HttpContext.User.Claims)
      {
        if(claim.Type.Contains("tenantid"))
        {
          oidcAuthenticationScheme = "aad";
          break;
        }
      }
      return new SignOutResult(new[]
      {
        oidcAuthenticationScheme,
        CookieAuthenticationDefaults.AuthenticationScheme
      });
    }
  }
}
