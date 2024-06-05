using IdentityJWT.Models;
using IdentityJWT.Models.DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {}
        

        public DbSet<JWTRefreshToken> JwtRefreshToken { get; set; }
        public DbSet<JwtBlacklistToken> JwtBlacklistToken { get; set; }
        public DbSet<EnterpriseLogging> EnterpriseLogging { get; set; }


    }
}
