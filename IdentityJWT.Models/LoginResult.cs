using IdentityJWT.Models.DTO;
using IdentityJWT.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.Models
{
    public class LoginResult
    {
        public LoginResult() { }

        public LoginResult(ApplicationUser user, string token, string refreshToken)
        {
            LoggedIn = true;
            User = new UserVM
            {
                Fname = user.Fname,
                Lname = user.Lname,
                Email = user.Email ?? "",
                Id = user.Id
            };
            Token = token;
            RefreshToken = refreshToken;
        }

        public bool LoggedIn { get; set; } = false;

        public UserVM? User { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
