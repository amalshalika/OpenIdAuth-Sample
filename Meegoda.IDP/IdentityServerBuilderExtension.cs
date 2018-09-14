using IdentityServer4.Services;
using Meegoda.IDP.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Meegoda.IDP
{
    public static class IdentityServerBuilderExtension
    {
        public static IIdentityServerBuilder AddToDoUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<ITodoUserRepository, TodoUserRepository>();
            builder.Services.AddScoped<IProfileService, ToDoUserProfileService>();
            return builder;
        }
    }
}
