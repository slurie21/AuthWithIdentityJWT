using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;
using IdentityJWT.Utility.Interface;
using IdentityJWT.Models.DTO;
using IdentityJWT.Utility;

namespace IdentityJWT.UnitTests
{
    public class JwtManagerTests
    {
        private readonly IJwtManager _jwtManager;
        private readonly Mock<IConfiguration> _configurationMock;

        public JwtManagerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.SetupGet(c => c["JWT_Secret"]).Returns("6493e781134dc34fec23bcf2e98a67d804b639821067cc2b8dd4c3979371dc33");
            _configurationMock.SetupGet(c => c["JWT_Refresh_Secret"]).Returns("c280176176b18ede58a06ec98e326ad6618db207ebbbf76224fbb84b6c3ec832");

            _jwtManager = new JwtManager(_configurationMock.Object);
        }

        [Fact]
        public void GenerateJwtToken_WithUserOnly_ReturnsValidToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "123", UserName = "john.doe", Email = "john.doe@example.com", Fname = "John", Lname = "Doe" };

            // Act
            string token = _jwtManager.GenerateJwtToken(user, null);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().BeOfType<string>();
        }

        [Fact]
        public void GenerateJwtToken_WithUserAndRoles_ReturnsValidToken()
        {
            // Arrange
            var user = new ApplicationUser { Id = "123", UserName = "john.doe", Email = "john.doe@example.com", Fname = "John", Lname = "Doe" };
            var roles = new List<string> { "Admin", "User" };

            // Act
            string token = _jwtManager.GenerateJwtToken(user, roles);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().BeOfType<string>();
        }
    }
}
