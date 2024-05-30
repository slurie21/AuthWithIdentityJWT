using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.Models
{
    public class LoginResult
    {
        public bool LoggedIn { get; set; } = false;
        public string JwtToken { get; set; }
        public string JwtRefreshToken { get; set; }
    }
}
