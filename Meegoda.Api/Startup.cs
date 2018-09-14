using IdentityServer4.AccessTokenValidation;
using Meegoda.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Meegoda.Api
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
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44377/";
                    options.RequireHttpsMetadata = true;
                    options.ApiName = "usertodoapi";
                    options.ApiSecret = "apisecret";
                });

            services.AddAuthorization(authorizationConfig =>
            {
                authorizationConfig.AddPolicy("MustOwnTodo", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new MustOwnTodoRequirment());
                });
            });
            services.AddSingleton<IAuthorizationHandler, MustOwnTodoHandler>();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();

            app.UseMvc();
        }
    }
}
