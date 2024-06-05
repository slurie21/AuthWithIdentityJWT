using IdentityJWT.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IdentityJWT.Models.DTO
{
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser() { }

        public ApplicationUser(UserVM userVM) 
        {
            Fname = userVM.Fname;
            Lname = userVM.Lname;
            Email = userVM.Email;
            CreatedDate = DateTime.UtcNow;   
        }
        public ApplicationUser(RegistrationVM registerVM)
        {
            Fname = registerVM.Fname;
            Lname = registerVM.Lname;
            Email = registerVM.Email;
            UserName = registerVM.Email;
            CreatedDate = DateTime.UtcNow;
            EmailConfirmed = true; //remove once email piece in place
        }

        public bool IsActive { get; set; } = true;
        public string Fname { get; set; }
        public string Lname { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedAt { get; set; }

        public UserVM GetUserVM()
        {
            return new UserVM
            {
                Email = this.Email,
                Fname = this.Fname,
                Lname = this.Lname,
                Id = this.Id
            };
        }

    }
}
