using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace IdentityJWT.Models.ViewModels
{
    public class RegistrationVM : UserVM
    {
        [JsonProperty]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        public UserVM GetUserVM()
        {
            return new UserVM
            {
                Id = this.Id,
                Fname = this.Fname,
                Lname = this.Lname,
                Email = this.Email
            };
        }
    }
}
