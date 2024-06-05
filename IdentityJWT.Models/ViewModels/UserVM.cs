using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.Models.ViewModels
{
    public class UserVM
    {

        public string? Id { get; set; }

        [Required(ErrorMessage ="First Name must be supplied")]
        public string Fname { get; set; }
        [Required(ErrorMessage = "Last Name must be supplied")]
        public string Lname { get; set; }
        [Required(ErrorMessage = "Email must be supplied")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username must be supplied")]
        public string UserName {  get; set; }
    }
}