using System;
using Xunit;
using Backend.Services;
using Microsoft.Extensions.Configuration;
using Backend.Models;
using System.Collections.Generic;

namespace BackendTest.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateToken_WhenCalled_ReturnsTokenString()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, String>
            {
                {"AuthKey", "tokenservice-mock-jwt-key"}
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var tokenService = new TokenService(configuration);
            var user = new User()
            {
                Name = "UserTest",
                Login = "LoginTest",
                Password = "PasswordTest",
                Email = "email@test",
                Birthday = DateTime.MinValue,
                Cpf = "CpfTest"
            };
            // Act
            var token = tokenService.GenerateToken(user);
            // Assert
            Assert.IsType<String>(token);
        }
    }
}