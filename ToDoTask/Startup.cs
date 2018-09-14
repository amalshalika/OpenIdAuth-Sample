using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ToDoTask
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
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:44377/";
                options.RequireHttpsMetadata = true;
                options.ClientId = "ToDoTaskManagmentClient";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("address");
                options.Scope.Add("roles");
                options.Scope.Add("usertodoapi");
                options.Scope.Add("countries");
                options.Scope.Add("subscriptionlevel");
                options.Scope.Add("offline_access");
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.ClientSecret = "secret";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey("given_name", "given_name");
                options.ClaimActions.MapJsonKey("family_name", "family_name");
                options.ClaimActions.MapJsonKey("role", "role");
                options.ClaimActions.MapJsonKey("country", "country");
                options.ClaimActions.MapJsonKey("subscriptionlevel", "subscriptionlevel");
                options.Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = e =>
                    {
                        var identity = e.Principal;
                        var subjectClaim = identity.Claims.FirstOrDefault(z => z.Type == "sub");
                        var expClaims = identity.Claims.FirstOrDefault(z => z.Type == "exp");
                        var newClaimsIdentity = new ClaimsIdentity(e.Scheme.Name);
                        newClaimsIdentity.AddClaim(subjectClaim);
                        newClaimsIdentity.AddClaim(expClaims);

                        e.Principal = new ClaimsPrincipal(newClaimsIdentity);
                        return Task.FromResult(0);
                    },
                    OnUserInformationReceived = e =>
                    {
                        e.User.Remove("address");
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddAuthorization(autorizationOptions =>
            {
                autorizationOptions.AddPolicy("CanOrderFrame",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "sl");
                        policyBuilder.RequireClaim("subscriptionlevel", "payinguser");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=ToDo}/{action=Index}/{id?}");
            });
        }
    }
}
