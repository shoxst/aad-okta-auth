using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace UserGroupRole
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(options =>
      {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
      })
      .AddCookie()
      .AddOpenIdConnect("aad", options =>    
      {
        options.Authority = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"] + "/v2.0";
        options.ClientId = Configuration["AzureAd:ClientId"];
        options.ResponseType = OpenIdConnectResponseType.IdToken;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "name",
          RoleClaimType = "role",
          ValidateIssuer = true
        };
        options.CallbackPath = Configuration["AzureAd:CallbackPath"];
        options.SignedOutCallbackPath = Configuration["AzureAd:SingnedOutCallbackPath"];
      })
      .AddOpenIdConnect("okta", options =>
      {
        options.Authority = Configuration["Okta:Domain"] + "/oauth2/default";
        options.ClientId = Configuration["Okta:ClientId"];
        options.ResponseType = OpenIdConnectResponseType.IdToken;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "name",
          RoleClaimType = "groups",
          ValidateIssuer = true
        };
        options.SaveTokens = true;
        options.CallbackPath = Configuration["Okta:CallbackPath"];
        options.SignedOutCallbackPath = Configuration["Okta:SingnedOutCallbackPath"];
      });
      

      services.AddAuthorization();
      services.AddControllersWithViews();

      services.AddRazorPages();
      services.AddSingleton(SampleData.Initialize());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
      });
    }
  }
}
