using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meegoda.IDP.Controllers.UserRegistration
{
    public class RegistrationInputModel
    {
        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }

    }
}
