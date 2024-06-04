using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess.IRepo
{
    public interface IUnitOfWork
    {
        IJWTRefreshRepo JwtRefresh { get; }
        IEnterpriseLoggingRepo EnterpriseLogging { get; }
        Task Save();
    }
}
