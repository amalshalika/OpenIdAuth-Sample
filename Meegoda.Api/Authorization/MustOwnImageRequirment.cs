using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meegoda.Api.Authorization
{
    public class MustOwnTodoRequirment:IAuthorizationRequirement
    {
    }
}
