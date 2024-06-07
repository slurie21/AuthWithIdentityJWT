using IdentityJWT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess.IRepo
{
    public interface IJWTRefreshRepo : IRepository<JWTRefreshToken>
    {
        Task<int> DeleteAllRefreshTokensByUserID(string userId);

    }
}
