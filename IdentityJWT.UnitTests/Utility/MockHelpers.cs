using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityJWT.UnitTests.Utility
{
    public static class MockHelpers{

            public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
            {
                var store = new Mock<IUserStore<TUser>>();
                var options = new Mock<IOptions<IdentityOptions>>();
                var passwordHasher = new Mock<IPasswordHasher<TUser>>();
                var userValidators = new List<IUserValidator<TUser>> { new Mock<IUserValidator<TUser>>().Object };
                var passwordValidators = new List<IPasswordValidator<TUser>> { new Mock<IPasswordValidator<TUser>>().Object };
                var keyNormalizer = new Mock<ILookupNormalizer>();
                var errors = new Mock<IdentityErrorDescriber>();
                var services = new Mock<IServiceProvider>();
                var logger = new Mock<ILogger<UserManager<TUser>>>();

                return new Mock<UserManager<TUser>>(
                    store.Object, options.Object, passwordHasher.Object, userValidators, passwordValidators, keyNormalizer.Object, errors.Object, services.Object, logger.Object);
            }
    }    
}
