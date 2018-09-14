using IdentityServer4.Services;
using Meegoda.IDP.Entities;
using Meegoda.IDP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Meegoda.IDP.Controllers.UserRegistration
{
    public class UserRegistrationController : Controller
    {
        private readonly ITodoUserRepository todoUserRepository;
        private readonly IIdentityServerInteractionService identityServerInteractionService;

        public UserRegistrationController(ITodoUserRepository totoUserRepository, IIdentityServerInteractionService identityServerInteractionService)
        {
            this.todoUserRepository = totoUserRepository;
            this.identityServerInteractionService = identityServerInteractionService;
        }
        [HttpGet]
        public IActionResult RegisterUser(RegistrationInputModel registrationInputModel)
        {
            var vm = new RegisterUserViewModel()
            {
                ReturnUrl = registrationInputModel.ReturnUrl,
                Provider = registrationInputModel.Provider,
                ProviderUserId = registrationInputModel.ProviderUserId
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userToCreate = new User();
            userToCreate.Username = model.Username;
            userToCreate.Password = model.Password;
            userToCreate.IsActive = true;

            userToCreate.Claims.Add(new UserClaim("country", model.Country));
            userToCreate.Claims.Add(new UserClaim("address", model.Address));
            userToCreate.Claims.Add(new UserClaim("given_name", model.Firstname));
            userToCreate.Claims.Add(new UserClaim("family_name", model.Lastname));
            userToCreate.Claims.Add(new UserClaim("email", model.Email));
            userToCreate.Claims.Add(new UserClaim("subscriptionLevel", "FreeUser"));

            todoUserRepository.AddUser(userToCreate);

            if (model.IsProvisioningFromExternal)
            {
                userToCreate.Logins.Add(new UserLogin()
                {
                    LoginProvider = model.Provider,
                    ProviderKey = model.ProviderUserId
                });
            }

            if (!todoUserRepository.Save())
            {
                throw new Exception($"creating a user failed");
            }

            if (!model.IsProvisioningFromExternal)
            {
                await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username);
            }

            if (identityServerInteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("~/");
        }
    }
}
