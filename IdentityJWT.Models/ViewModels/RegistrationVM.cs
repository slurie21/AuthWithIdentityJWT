using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace IdentityJWT.Models.ViewModels
{
    public class RegistrationVM : UserVM
    {
        [JsonProperty]
        [Required]
        public string Password { get; set; }

    }
}
