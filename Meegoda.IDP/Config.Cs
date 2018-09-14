using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Meegoda.IDP
{
    public class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "97ee12ac-88e7-462a-86fa-2e92aa97e603",
                    Username = "john",
                    Password = "amal",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Uderwood"),
                        new Claim("address", "Udagewattha, Meegoda"),
                        new Claim("role", "freeuser"),
                        new Claim("country","sl"),
                        new Claim("subscriptionlevel","payinguser")

                    }
                },
                new TestUser
                {
                    SubjectId = "b9537179-7766-44a9-8f72-bb0a1ac982bf",
                    Username = "finn",
                    Password = "amal",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Chamil"),
                        new Claim("family_name", "Chinthaka"),
                        new Claim("address", "Padukka, Kotagoda"),
                        new Claim("role", "payinguser"),
                        new Claim("country","in"),
                        new Claim("subscriptionlevel","payinguser")
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your roles", new List<string>(){"role"}),
                new IdentityResource("countries","your countries",new List<string> {"country"}),
                new IdentityResource("subscriptionlevel","your subcription level",new List<string> {"subscriptionlevel"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("usertodoapi","user todo api", new List<string>() {"role"})
                {
                    ApiSecrets = { new Secret("apisecret".Sha256()) }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientName = "Todo Task Managment",
                    ClientId = "ToDoTaskManagmentClient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44326/signin-oidc"
                    },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "usertodoapi",
                        "countries",
                        "subscriptionlevel"

                    },
                    AccessTokenLifetime = 30,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44326/signout-callback-oidc"
                    }
                    //AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }
    }
}
