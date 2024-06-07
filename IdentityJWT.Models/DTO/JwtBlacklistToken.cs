using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.Models.DTO
{
    public class JwtBlacklistToken
    {
        public JwtBlacklistToken() { }

        public JwtBlacklistToken(string jti, string token, string userID, DateTime createdAt, DateTime expiresAt)
        {
            Jti = jti;
            Token = token;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            UserId = userID;
        }


        public int Id { get; set; }
        public string Jti { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

