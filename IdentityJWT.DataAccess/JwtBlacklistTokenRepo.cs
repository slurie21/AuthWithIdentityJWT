using IdentityJWT.DataAccess.Context;
using IdentityJWT.DataAccess.IRepo;
using IdentityJWT.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess
{
    public class JwtBlacklistTokenRepo : Repository<JwtBlacklistToken>, IJwtBlacklistTokenRepo
    {
        private AppDbContext _db;

        public JwtBlacklistTokenRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
