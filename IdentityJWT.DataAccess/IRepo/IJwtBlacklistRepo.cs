using IdentityJWT.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess.IRepo
{
    public interface IJwtBlacklistTokenRepo : IRepository<JwtBlacklistToken>
    {
    }
}
