using IdentityJWT.DataAccess.Context;
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

        public IJWTRefreshRepo JwtRefreshToken { get; private set; }

        public IEnterpriseLoggingRepo EnterpriseLogging { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            JwtRefreshToken = new JWTRefreshRepo(_db);
            EnterpriseLogging = new EnterpriseLoggingRepo(_db);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
