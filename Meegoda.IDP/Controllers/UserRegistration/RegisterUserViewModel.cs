using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Meegoda.IDP.Controllers.UserRegistration
{
    public class RegisterUserViewModel
    {
        [MaxLength(100)]
        public string Username { get; set; }
        [MaxLength(100)]
        public string Password { get; set; }
        [MaxLength(100)]
        public string Firstname { get; set; }
        [MaxLength(100)]
        public string Lastname { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }

        //public SelectList CountryCodes { get; set; } = new SelectList(new[] {
        //    new { Id = "BE", Value = "Belgium" },
        //    new { Id = "US", Value = "United State" },
        //    new { Id = "SL", Value = "Sri Lanka" }
        //});

        public List<SelectListItem> CountryCodes { get; set; } = new List<SelectListItem>
    {
        new SelectListItem {Value = "BE", Text = "Belgium"},
        new SelectListItem {Value = "US", Text = "United State"},
        new SelectListItem {Value = "SL", Text = "Sri Lanka"}
    };

        public string ReturnUrl { get; set; }

        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        public bool IsProvisioningFromExternal
        {
            get
            {
                return (Provider != null);
            }
        }


    }
}
