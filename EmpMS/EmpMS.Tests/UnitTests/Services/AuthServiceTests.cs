using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly Mock<IEmployeeRepository> _mockEmpRepo;
        private readonly Mock<IJwtHelper> _mockJwt;
        private readonly Mock<IEmailService> _mockEmail;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly IMemoryCache _cache;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockEmpRepo = new Mock<IEmployeeRepository>();
            _mockJwt = new Mock<IJwtHelper>();
            _mockEmail = new Mock<IEmailService>();
            _mockUoW = new Mock<IUnitOfWork>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new AuthService(
                _mockAuthRepo.Object, _mockRoleRepo.Object, _mockEmpRepo.Object,
                _mockJwt.Object, _cache, _mockEmail.Object, _mockUoW.Object);
        }

        [Fact]
        public async Task CreateUser_DuplicateUsername_ThrowsBadRequest()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("admin")).ReturnsAsync(true);
            var dto = new CreateUserDto { Username = "admin", Email = "a@t.com", RoleIds = new List<int> { 1 } };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateUserAsync(dto, "system"));
        }

        [Fact]
        public async Task CreateUser_DuplicateEmail_ThrowsBadRequest()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("u")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("e@t.com")).ReturnsAsync(true);
            var dto = new CreateUserDto { Username = "u", Email = "e@t.com", RoleIds = new List<int> { 1 } };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateUserAsync(dto, "system"));
        }

        [Fact]
        public async Task CreateUser_NoRoles_ThrowsBadRequest()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("u")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("e@t.com")).ReturnsAsync(false);
            var dto = new CreateUserDto { Username = "u", Email = "e@t.com", RoleIds = new List<int>() };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateUserAsync(dto, "system"));
        }

        [Fact]
        public async Task CreateUser_InvalidEmployee_ThrowsNotFound()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("u")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("e@t.com")).ReturnsAsync(false);
            _mockEmpRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee)null);
            var dto = new CreateUserDto { Username = "u", Email = "e@t.com", RoleIds = new List<int> { 1 }, EmployeeId = 999 };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateUserAsync(dto, "system"));
        }

        [Fact]
        public async Task CreateUser_EmployeeAlreadyHasUser_ThrowsBadRequest()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("u")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("e@t.com")).ReturnsAsync(false);
            _mockEmpRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1 });
            _mockAuthRepo.Setup(r => r.EmployeeHasUserAsync(1)).ReturnsAsync(true);
            var dto = new CreateUserDto { Username = "u", Email = "e@t.com", RoleIds = new List<int> { 1 }, EmployeeId = 1 };
            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateUserAsync(dto, "system"));
        }

        [Fact]
        public async Task Login_UserNotFound_ThrowsBadRequest()
        {
            _mockAuthRepo.Setup(r => r.GetUserByUsernameAsync("bad")).ReturnsAsync((User)null);
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.LoginAsync(new LoginDto { Username = "bad", Password = "p" }));
        }

        [Fact]
        public async Task Login_WrongPassword_ThrowsUnauthorized()
        {
            var user = new User { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") };
            _mockAuthRepo.Setup(r => r.GetUserByUsernameAsync("admin")).ReturnsAsync(user);
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.LoginAsync(new LoginDto { Username = "admin", Password = "wrong" }));
        }

        [Fact]
        public async Task Login_Valid_ReturnsTokens()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Pass@1");
            var user = new User { Id = 1, Username = "admin", Email = "a@t.com", PasswordHash = hash };
            _mockAuthRepo.Setup(r => r.GetUserByUsernameAsync("admin")).ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.GetUserPermissionsAsync(1)).ReturnsAsync(new List<string> { "Read" });
            _mockJwt.Setup(j => j.GenerateToken(1, "admin", "a@t.com", null, It.IsAny<List<string>>())).Returns("at");
            _mockJwt.Setup(j => j.GenerateRefreshToken()).Returns("rt");

            var result = await _service.LoginAsync(new LoginDto { Username = "admin", Password = "Pass@1" });

            Assert.Equal("at", result.Token);
            Assert.Equal("rt", result.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_InvalidToken_ThrowsUnauthorized()
        {
            _mockJwt.Setup(j => j.GetPrincipalFromExpiredToken("bad")).Returns((ClaimsPrincipal)null);
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.RefreshTokenAsync(new RefreshTokenDto { AccessToken = "bad", RefreshToken = "rt" }));
        }

        [Fact]
        public async Task SendOtp_EmptyEmail_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.SendOtpAsync(new ForgotPasswordDto { Email = "" }));
        }

        [Fact]
        public async Task SendOtp_UserNotFound_ThrowsNotFound()
        {
            _mockAuthRepo.Setup(r => r.GetUserByEmailAsync("bad@t.com")).ReturnsAsync((User)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.SendOtpAsync(new ForgotPasswordDto { Email = "bad@t.com" }));
        }

        [Fact]
        public async Task SendOtp_Valid_SendsEmail()
        {
            _mockAuthRepo.Setup(r => r.GetUserByEmailAsync("t@t.com")).ReturnsAsync(new User { Id = 1, Email = "t@t.com" });
            await _service.SendOtpAsync(new ForgotPasswordDto { Email = "t@t.com" });
            _mockEmail.Verify(e => e.SendEmailAsync("t@t.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_EmptyOtp_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.ResetPasswordAsync(new ResetPasswordDto { Otp = "", NewPassword = "p", Email = "t@t.com" }));
        }

        [Fact]
        public async Task ResetPassword_EmptyPassword_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.ResetPasswordAsync(new ResetPasswordDto { Otp = "123456", NewPassword = "", Email = "t@t.com" }));
        }

        [Fact]
        public async Task ChangePassword_UserNotFound_ThrowsNotFound()
        {
            _mockAuthRepo.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User)null);
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.ChangePasswordAsync(999, new ChangePasswordDto()));
        }

        [Fact]
        public async Task ChangePassword_WrongOld_ThrowsBadRequest()
        {
            var user = new User { Id = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct") };
            _mockAuthRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.ChangePasswordAsync(1, new ChangePasswordDto { OldPassword = "wrong", NewPassword = "new" }));
        }

        [Fact]
        public async Task ChangePassword_Valid_UpdatesAndSaves()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Old@1");
            var user = new User { Id = 1, PasswordHash = hash, MustChangePassword = true };
            _mockAuthRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            await _service.ChangePasswordAsync(1, new ChangePasswordDto { OldPassword = "Old@1", NewPassword = "New@1" });

            Assert.False(user.MustChangePassword);
            _mockAuthRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // CreateUser Full Happy Path

        [Fact]
        public async Task CreateUser_Valid_CreatesUserWithHashedPasswordAndSendsEmail()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("newuser")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("new@t.com")).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1, RoleName = "Employee" });

            User capturedUser = null;
            _mockAuthRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);

            var dto = new CreateUserDto { Username = "newuser", Email = "new@t.com", RoleIds = new List<int> { 1 } };
            var result = await _service.CreateUserAsync(dto, "admin");

            Assert.NotNull(capturedUser);
            Assert.Equal("newuser", capturedUser.Username);
            Assert.Equal("new@t.com", capturedUser.Email);
            Assert.True(capturedUser.IsActive);
            Assert.True(capturedUser.MustChangePassword);
            Assert.NotEmpty(capturedUser.PasswordHash);
            Assert.NotEqual(capturedUser.PasswordHash, "plaintext");

            _mockEmail.Verify(e => e.SendEmailAsync("new@t.com", It.IsAny<string>(), It.Is<string>(body => body.Contains("newuser"))), Times.Once);
            _mockUoW.Verify(u => u.Commit(), Times.Once);
        }

        // Login Saves Refresh Token

        [Fact]
        public async Task Login_Valid_SavesRefreshTokenToDatabase()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Pass@1");
            var user = new User { Id = 1, Username = "admin", Email = "a@t.com", PasswordHash = hash };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameAsync("admin")).ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.GetUserPermissionsAsync(1)).ReturnsAsync(new List<string>());
            _mockJwt.Setup(j => j.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<List<string>>())).Returns("at");
            _mockJwt.Setup(j => j.GenerateRefreshToken()).Returns("new-refresh-token");

            await _service.LoginAsync(new LoginDto { Username = "admin", Password = "Pass@1" });

            Assert.Equal("new-refresh-token", user.RefreshToken);
            Assert.NotNull(user.RefreshTokenExpiryTime);
            Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow.AddDays(6));
            _mockAuthRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
        }

        // Login Returns MustChangePassword

        [Fact]
        public async Task Login_MustChangePasswordTrue_ReflectedInResponse()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("Pass@1");
            var user = new User { Id = 1, Username = "admin", Email = "a@t.com", PasswordHash = hash, MustChangePassword = true };

            _mockAuthRepo.Setup(r => r.GetUserByUsernameAsync("admin")).ReturnsAsync(user);
            _mockAuthRepo.Setup(r => r.GetUserPermissionsAsync(1)).ReturnsAsync(new List<string>());
            _mockJwt.Setup(j => j.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<List<string>>())).Returns("at");
            _mockJwt.Setup(j => j.GenerateRefreshToken()).Returns("rt");

            var result = await _service.LoginAsync(new LoginDto { Username = "admin", Password = "Pass@1" });

            Assert.True(result.MustChangePassword);
        }

        // Transaction Rollback

        [Fact]
        public async Task CreateUser_ExceptionDuringCreate_RollsBackTransaction()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("u")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("e@t.com")).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1, RoleName = "Admin" });
            _mockAuthRepo.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ThrowsAsync(new Exception("DB error"));

            var dto = new CreateUserDto { Username = "u", Email = "e@t.com", RoleIds = new List<int> { 1 } };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateUserAsync(dto, "system"));

            _mockUoW.Verify(u => u.Rollback(), Times.Once);
        }

        // OTP Cache Validation Flow

        [Fact]
        public async Task ResetPassword_ValidOtp_UpdatesPasswordAndClearsCache()
        {
            _cache.Set("test@t.com", "123456", new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            var user = new User { Id = 1, Email = "test@t.com", PasswordHash = "old-hash" };
            _mockAuthRepo.Setup(r => r.GetUserByEmailAsync("test@t.com")).ReturnsAsync(user);

            await _service.ResetPasswordAsync(new ResetPasswordDto { Email = "test@t.com", Otp = "123456", NewPassword = "NewPass@1" });

            Assert.NotEqual("old-hash", user.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("NewPass@1", user.PasswordHash));
            _mockAuthRepo.Verify(r => r.UpdateUserAsync(user), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.False(_cache.TryGetValue("test@t.com", out _));
        }

        [Fact]
        public async Task ResetPassword_WrongOtp_ThrowsBadRequest()
        {
            _cache.Set("test@t.com", "123456", new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.ResetPasswordAsync(new ResetPasswordDto { Email = "test@t.com", Otp = "999999", NewPassword = "NewPass@1" }));
        }

        // CreateUser Role Assignment

        [Fact]
        public async Task CreateUser_MultipleRoles_AssignsAllRoles()
        {
            _mockAuthRepo.Setup(r => r.UserExistAsync("user")).ReturnsAsync(false);
            _mockAuthRepo.Setup(r => r.EmailExistsAsync("u@t.com")).ReturnsAsync(false);
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(1)).ReturnsAsync(new Role { Id = 1, RoleName = "Admin" });
            _mockRoleRepo.Setup(r => r.GetRoleByIdAsync(2)).ReturnsAsync(new Role { Id = 2, RoleName = "HR" });

            var capturedRoles = new List<UserRole>();
            _mockAuthRepo.Setup(r => r.AddUserRoleAsync(It.IsAny<UserRole>()))
                .Callback<UserRole>(ur => capturedRoles.Add(ur))
                .Returns(Task.CompletedTask);

            var dto = new CreateUserDto { Username = "user", Email = "u@t.com", RoleIds = new List<int> { 1, 2 } };
            var result = await _service.CreateUserAsync(dto, "admin");

            Assert.Equal(2, capturedRoles.Count);
            Assert.Contains("Admin", result.Roles);
            Assert.Contains("HR", result.Roles);
        }
    }
}


