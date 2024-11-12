using AuthServer.Errors;
using AuthServer.Roles;
using AuthServer.Security;
using AuthServer.Users;
using FluentAssertions;
using Moq;
using Xunit;

namespace AuthServer.Testes
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly UserService _service;

        public UserServiceTest()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _service = new UserService(_repositoryMock.Object, _roleRepositoryMock.Object, _jwtServiceMock.Object, null);
        }

        [Fact]
        public void Insert_ThrowsBadRequestException_IfEmailExists()
        {
            // Arrange
            var existingUser = new User("John Doe", "user@email.com", "password");
            _repositoryMock.Setup(repo => repo.FindByEmail(existingUser.Email)).ReturnsAsync(existingUser);

            // Act
            var act = () => _service.Insert(existingUser);

            // Assert
            act.Should().ThrowAsync<BadRequestException>().WithMessage("Email already exists.");
        }

        [Fact]
        public async Task Insert_SavesData_IfEmailDoesNotExist()
        {
            // Arrange
            var userToInsert = new User("John Doe", "newuser@email.com", "password");
            var savedUser = new User("John Doe", "newuser@email.com", "password") { Id = 1 };
            _repositoryMock.Setup(repo => repo.FindByEmail(userToInsert.Email)).ReturnsAsync((User)null);
            _repositoryMock.Setup(repo => repo.Insert(userToInsert)).ReturnsAsync(savedUser);

            // Act
            var result = await _service.Insert(userToInsert);

            // Assert
            result.Should().BeEquivalentTo(savedUser);
        }

        [Fact]
        public void Insert_ThrowsIllegalArgumentException_IfUserHasId()
        {
            // Arrange
            var userWithId = new User("John Doe", "user@email.com", "password") { Id = 1 };

            // Act
            var act = () => _service.Insert(userWithId);

            // Assert
            act.Should().ThrowAsync<ArgumentException>().WithMessage("User cannot have an ID before insertion.");
        }
    }
}