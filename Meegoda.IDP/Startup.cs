using IdentityServer4;
using IdentityServer4.Services;
using Meegoda.IDP.Entities;
using Meegoda.IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meegoda.IDP
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoUserContext>(options => options.UseSqlServer(Configuration.GetConnectionString("UserDbConnectionString")));
            services.AddScoped<ITodoUserRepository, TodoUserRepository>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddToDoUserStore()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());

            services.AddAuthentication("MyCookie")
            .AddCookie("MyCookie", options =>
            {
                options.ExpireTimeSpan = new System.TimeSpan(0, 0, 15);
            });

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddAuthentication()
           .AddGoogle("Google", options =>
           {
               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

               options.ClientId = "655511605746-op6jhbqjdtkfiu3iv84uj10dj3qrde57.apps.googleusercontent.com";
               options.ClientSecret = "3qOm35B4_TpptLHooSqhPDBp";
           });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TodoUserContext userContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //userContext.Database.Migrate();
            //userContext.EnsureSeedDataForContext();

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
