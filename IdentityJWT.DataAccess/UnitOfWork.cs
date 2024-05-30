using IdentityJWT.DataAccess.IRepo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;

        public IJWTRefreshRepo JwtRefresh { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            JwtRefresh = new JWTRefreshRepo(_db);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
