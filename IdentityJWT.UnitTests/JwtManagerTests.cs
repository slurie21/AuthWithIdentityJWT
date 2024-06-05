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
            _configurationMock.SetupGet(c => c["JWT_Secret"]).Returns("supersecretkey12345");
            _configurationMock.SetupGet(c => c["JWT_Refresh_Secret"]).Returns("supersecretrefreshkey12345");

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
