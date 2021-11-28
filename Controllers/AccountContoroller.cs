using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace DockerPipelineExample.Controllers
{
  public class AccountController : Controller
  {   
    public async Task<IActionResult> LoginWithAzure()
    {
      if(!HttpContext.User.Identity.IsAuthenticated)
      {
        return Challenge("aad");
      }

      var id_token = await HttpContext.GetTokenAsync("id_token");
      var accessToken = await HttpContext.GetTokenAsync("access_token");
      
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
