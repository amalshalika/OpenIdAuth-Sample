using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Meegoda.IDP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Meegoda.IDP.Services
{
    public class ToDoUserProfileService : IProfileService
    {
        private readonly ITodoUserRepository todoUserRepository;

        public ToDoUserProfileService(ITodoUserRepository todoUserRepository)
        {
            this.todoUserRepository = todoUserRepository;
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            IList<UserClaim> userClaims = todoUserRepository.GetUserClaimsBySubjectId(subjectId).ToList();
            context.IssuedClaims = userClaims.Select(uc => new Claim(uc.ClaimType, uc.ClaimValue)).ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = todoUserRepository.IsUserActive(subjectId);
            return Task.CompletedTask;
        }
    }
}
