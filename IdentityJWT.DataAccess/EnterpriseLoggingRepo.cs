using IdentityJWT.DataAccess.Context;
using IdentityJWT.DataAccess.IRepo;
using IdentityJWT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess
{
    public class EnterpriseLoggingRepo : Repository<EnterpriseLogging>, IEnterpriseLoggingRepo
    {
        private AppDbContext _db;

        public EnterpriseLoggingRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
