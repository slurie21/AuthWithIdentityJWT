using IdentityJWT.Models.DTO;
using IdentityJWT.Models.ViewModels;
using IdentityJWT.Models;
using IdentityJWT.DataAccess.IRepo;
using IdentityJWT.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityJWT.UnitTests.Utility;
using FluentAssertions;

namespace IdentityJWT.UnitTests
{

    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly LogMessageCapturer<AccountController> _logMessageCapturer;
        private readonly AccountController _controller;
        private readonly DefaultHttpContext _httpContext;

        public AccountControllerTests()
        {
            _mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _logMessageCapturer = new LogMessageCapturer<AccountController>();
            _controller = new AccountController(_mockUserManager.Object, _mockUnitOfWork.Object, _logMessageCapturer);
            
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext = _httpContext;
        }

        [Fact]
        public async Task Register_SuccessfulUserCreation_ReturnsCreated()
        {
            // Arrange
            var registrationVM = new RegistrationVM { Email = "john.doe@example.com", Fname = "John", Lname = "Doe" };
            _httpContext.Items["correlationID"] = Guid.NewGuid().ToString();

            var user = new ApplicationUser(registrationVM);
            var identityResult = IdentityResult.Success;

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(identityResult);
            _mockUnitOfWork.Setup(uow => uow.EnterpriseLogging.Add(It.IsAny<EnterpriseLogging>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.Save()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.Register(registrationVM);


            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var userVM = Assert.IsType<UserVM>(createdResult.Value);

            _logMessageCapturer.Messages.Should().ContainSingle(message => message.Contains("New user John Doe created successfully."));
            _mockUnitOfWork.Verify(uow => uow.EnterpriseLogging.Add(It.IsAny<EnterpriseLogging>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public async Task Register_UserCreationFails_ReturnsBadRequest()
        {
            // Arrange
            var registrationVM = new RegistrationVM { Email = "john.doe@example.com", Fname = "John", Lname = "Doe" };
            _httpContext.Items["correlationID"] = Guid.NewGuid().ToString();

            var user = new ApplicationUser(registrationVM);
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Test error" });

            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(identityResult);
            _mockUnitOfWork.Setup(uow => uow.EnterpriseLogging.Add(It.IsAny<EnterpriseLogging>())).Verifiable();
            _mockUnitOfWork.Setup(uow => uow.Save()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.Register(registrationVM);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value;

            // Validate the error response content
            Assert.NotNull(errorResponse);

            // Extract errors and user from the anonymous type.  Need to also case it to an enuerable type
            var errors = (IEnumerable<IdentityError>)errorResponse.GetType().GetProperty("errors").GetValue(errorResponse);
            var userResponse = errorResponse.GetType().GetProperty("user").GetValue(errorResponse);
            
            Assert.Contains(errors, e => e.Description == "Test error");
            Assert.Equal(registrationVM, userResponse);

            _logMessageCapturer.Messages.Should().ContainSingle(message => message.Contains("User registration of John Doe had an issue."));
            _mockUnitOfWork.Verify(uow => uow.EnterpriseLogging.Add(It.IsAny<EnterpriseLogging>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }
    }

}
