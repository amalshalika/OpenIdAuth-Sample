using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ToDoTask.Models;

namespace ToDoTask.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        IHttpContextAccessor _contextAccessor;
        public ToDoController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();
            string accessToken = string.Empty;
            var todoList = new List<ToDoModel>();

            using (var httpClient = new HttpClient())
            {
                var expireAt = await _contextAccessor.HttpContext.GetTokenAsync("expires_at");
                if (string.IsNullOrWhiteSpace(expireAt) || DateTime.Parse(expireAt).ToUniversalTime() < DateTime.UtcNow)
                {
                    accessToken = await RenewToken();
                }
                else
                {
                    accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                }

                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.SetBearerToken(accessToken);
                    var response = await httpClient.GetStringAsync(new Uri(@"https://localhost:44389/api/usertodo"));

                    todoList = JsonConvert.DeserializeObject<List<ToDoModel>>(response);
                }
            }

            return View(todoList);
        }
        public async Task<IActionResult> Edit(int todoId)
        {
            ToDoEditViewModel todo;
            using (var httpClient = new HttpClient())
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.SetBearerToken(accessToken);
                }

                var response = await httpClient.GetStringAsync(new Uri($"https://localhost:44389/api/usertodo/{todoId}"));

                todo = JsonConvert.DeserializeObject<ToDoEditViewModel>(response);

            }
            return View("EditTodo", todo);
        }
        [HttpPost]
        public async Task<IActionResult> EditTodo(ToDoEditViewModel editTodo)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.SetBearerToken(accessToken);
                }
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                var stringContent = new StringContent(JsonConvert.SerializeObject(editTodo), Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(new Uri($"https://localhost:44389/api/usertodo/update/{editTodo.TodoId}"), stringContent).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized");
                }

                return RedirectToAction("index");

            }
        }
        public IActionResult Create()
        {
            return View("CreateTodo");
        }
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoViewModel createTodoViewModel)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.SetBearerToken(accessToken);
                }
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                var stringContent = new StringContent(JsonConvert.SerializeObject(createTodoViewModel), Encoding.UTF8, "application/json");

                var response = httpClient.PostAsync(new Uri($"https://localhost:44389/api/usertodo/create"), stringContent).Result;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized");
                }

                return RedirectToAction("index");

            }
        }
        public async Task<IActionResult> DeleteTodo(int todoId)
        {
            using (var httpClient = new HttpClient())
            {
                var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpClient.SetBearerToken(accessToken);
                }

                var response = httpClient.DeleteAsync(new Uri($"https://localhost:44389/api/usertodo/{todoId}")).Result;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Unauthorized");
                }

                return RedirectToAction("index");
            }
        }
        [Authorize(Policy = "CanOrderFrame")]
        public async Task<IActionResult> AddressInfo()
        {
            var discoveryClient = new DiscoveryClient("https://localhost:44377/");
            var doc = await discoveryClient.GetAsync();

            var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await userInfoClient.GetAsync(accessToken);

            if (response.IsError)
            {
                throw new Exception("Problem accessing userinfo endpoint", response.Exception);
            }

            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;

            return View(new AddressInfo(address));
        }
        public async Task Logout()
        {
            var discoveryResponse = await DiscoveryClient.GetAsync("https://localhost:44377/");
            var revocationClient = new TokenRevocationClient(discoveryResponse.RevocationEndpoint, "ToDoTaskManagmentClient", "secret");

            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var refreshToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            if(!string.IsNullOrEmpty(accessToken))
            {
                var revocationAcessTokenResponse = await revocationClient.RevokeAccessTokenAsync(accessToken);
                if(revocationAcessTokenResponse.IsError)
                {
                    throw new Exception("problems occur while revocation access token", revocationAcessTokenResponse.Exception);
                }
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var revocationRefreshResponse = await revocationClient.RevokeRefreshTokenAsync(refreshToken);
                if (revocationRefreshResponse.IsError)
                {
                    throw new Exception("problems occur while revocation refresh token", revocationRefreshResponse.Exception);
                }
            }

            await _contextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _contextAccessor.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        private async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
        private async Task<string> RenewToken()
        {
            var httpContext = _contextAccessor.HttpContext;
            var discoveryResponse = await DiscoveryClient.GetAsync("https://localhost:44377/");
            if (discoveryResponse.IsError)
            {
                throw new Exception(discoveryResponse.Error);
            }

            var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "ToDoTaskManagmentClient", "secret");
            // This will request a new access_token and a new refresh token.
            TokenResponse tokenResponse = await tokenClient.RequestRefreshTokenAsync(await httpContext.GetTokenAsync("refresh_token"));

            if (tokenResponse.IsError)
            {
                // Handle error.
            }

            var authenticationInfo = await httpContext.AuthenticateAsync("Cookies");
            //.GetAuthenticateInfoAsync("Cookies");
            var expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            authenticationInfo.Properties.ExpiresUtc = expiresAt;

            var oldIdToken = await httpContext.GetTokenAsync("id_token");
            var tokens = new List<AuthenticationToken> {
                            new AuthenticationToken
                            {
                                Name = OpenIdConnectParameterNames.IdToken,
                                Value = oldIdToken
                            },
                            new AuthenticationToken
                            {
                                Name = OpenIdConnectParameterNames.AccessToken,
                                Value = tokenResponse.AccessToken
                            },
                            new AuthenticationToken
                            {
                                Name = OpenIdConnectParameterNames.RefreshToken,
                                Value = tokenResponse.RefreshToken
                            }
                        };

            tokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
            });

            // Sign in the user with a new refresh_token and new access_token.
            authenticationInfo.Properties.StoreTokens(tokens);
            await httpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);

            return tokenResponse.AccessToken;
        }
    }
}