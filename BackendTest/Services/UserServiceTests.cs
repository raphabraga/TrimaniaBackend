using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Models.Exceptions;
using Backend.Services;
using BackendTest.Fixtures;
using Moq.AutoMock;
using Xunit;
using Moq;
using Backend.Repositories;
using Backend.Interfaces.Services;
using AutoMapper;

namespace BackendTest.Services
{
    public class UserServiceTests : IClassFixture<GenericFixture<UserService>>
    {
        private readonly GenericFixture<UserService> _fixture;

        public UserServiceTests(GenericFixture<UserService> fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task GetUserById_WhenAdministratorCalledWithAnExistingUserId_ReturnsSpecificUser(int userId)
        {
            // Given
            User requestingUser = new User()
            {
                Role = "Administrator"
            };
            // When
            User requestedUser = await _fixture.Service.GetUserById(requestingUser, userId);
            // Then
            Assert.Equal<User>(_fixture.Context.Users.Find(userId), requestedUser);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task
        GetUserById_WhenACustomerCalledWithItsUserId_ReturnsItsUser(int userId)
        {
            // Given
            User requestingUser = new User()
            {
                Id = userId,
                Role = "Customer"
            };
            // When
            User requestedUser = await _fixture.Service.GetUserById(requestingUser, userId);
            // Then
            Assert.Equal<User>(_fixture.Context.Users.Find(userId), requestedUser);
        }

        [Fact]
        public async Task
        GetUserById_WithUnexistingUserId_ThrowsRegisterNotFoundException()
        {
            // Given
            User requestingUser = new User();
            int userId = _fixture.Context.Users.Count() + 1;
            // When
            var act = async () => await _fixture.Service.GetUserById(requestingUser, userId);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task
        GetUserById_WhenCalledByCustomerWithNotItsOwnId_ThrowsUnauthorizedAccessException(int userId)
        {
            // Given
            User requestingUser = new User()
            {
                Id = userId,
                Role = "Customer",
            };
            // When
            var act = async () => await _fixture.Service.GetUserById(requestingUser, userId - 1);
            // Then
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task
        GetUserByLogin_WhenCalled_ReturnsUserWithSpecifiedLogin(int userId)
        {
            // Given
            string login = _fixture.Context.Users.Find(userId).Login;
            // When
            User user = await _fixture.Service.GetUserByLogin(login);
            // Then
            Assert.Equal<User>(_fixture.Context.Users.FirstOrDefault(u => u.Login == login), user);
        }

        [Fact]
        public async Task GetUserByLogin_WithNonExistingLogin_RegisterNotFoundException()
        {
            // Given
            string login = "NonExistingUserLogin";
            // When
            var act = async () => await _fixture.Service.GetUserByLogin(login);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData("asc")]
        [InlineData("desc")]
        public async Task GetUsers_WithSortingInSearchRequest_ReturnsAllUsersSorted(string sortBy)
        {
            // Given
            var searchRequest = new SearchUserRequest()
            {
                Filter = null,
                SortBy = sortBy,
                Page = null
            };
            // When
            var users = await _fixture.Service.GetUsers(searchRequest);
            // Then
            if (sortBy == "asc")
                Assert.Equal<List<User>>(_fixture.Context.Users.OrderBy(p => p.Name).ToList(), users);
            if (sortBy == "desc")
                Assert.Equal<List<User>>(_fixture.Context.Users.OrderByDescending(p => p.Name).ToList(), users);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public async Task GetUsers_WithPageInSearchRequest_ReturnsTenSpecificUsers(int page)
        {
            // Given
            var searchRequest = new SearchUserRequest()
            {
                Filter = null,
                SortBy = null,
                Page = page
            };
            // When
            var users = await _fixture.Service.GetUsers(searchRequest);
            // Then
            Assert.Equal<List<User>>(_fixture.Context.Users.Skip(10 * (page - 1)).Take(10).ToList(), users);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GetUsers_WithLoginInFilter_ReturnsFilteredUsersByLogin(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var loginFilter = user.Login.Substring(1, 5);
            var searchRequest = new SearchUserRequest()
            {
                Filter = loginFilter,
                SortBy = null,
                Page = null
            };
            // When
            var users = await _fixture.Service.GetUsers(searchRequest);
            // Then
            Assert.Equal<List<User>>(_fixture.Context.Users.Where(p => p.Login.Contains(loginFilter)).ToList(), users);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GetUsers_WithNameInFilter_ReturnsFilteredUsersByName(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var nameFilter = user.Name.Substring(1, 8);
            var searchRequest = new SearchUserRequest()
            {
                Filter = nameFilter,
                SortBy = null,
                Page = null
            };
            // When
            var users = await _fixture.Service.GetUsers(searchRequest);
            // Then
            Assert.Equal<List<User>>(_fixture.Context.Users.Where(p => p.Name.Contains(nameFilter)).ToList(), users);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GetUsers_WithEmailInFilter_ReturnsFilteredUsersByEmail(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var emailFilter = user.Email.Substring(1, 10);
            var searchRequest = new SearchUserRequest()
            {
                Filter = emailFilter,
                SortBy = null,
                Page = null
            };
            // When
            var users = await _fixture.Service.GetUsers(searchRequest);
            // Then
            Assert.Equal<List<User>>(_fixture.Context.Users.Where(p => p.Email.Contains(emailFilter)).ToList(), users);
        }

        [Fact]
        public async Task CreateUser_WhenCalled_CreateANewUserInDb()
        {
            // Given
            var user = new User
            {
                Id = _fixture.Context.Users.Count() + 1,
                Name = "New User",
                Login = "new_user",
                Email = "newuser@mail.com",
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = "NewUserCpf"
            };
            // When
            await _fixture.Service.CreateUser(user);
            // Then
            Assert.Equal<User>(_fixture.Context.Users.Find(user.Id), user);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task CreateUser_WithExistingLogin_ThrowsUsedLoginException(int userId)
        {
            // Given
            var user = new User
            {
                Id = _fixture.Context.Users.Count() + 1,
                Name = "New UserExistingLogin",
                Login = _fixture.Context.Users.Find(userId).Login,
                Email = "newuserexistinglogin@mail.com",
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = "NewUserExistingLoginCpf"
            };
            // When
            var act = async () => await _fixture.Service.CreateUser(user);
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(act);
            // Then
            Assert.True(aggregateException.InnerExceptions.Any(e => e is UsedLoginException));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task CreateUser_WithExistingEmail_ThrowsUsedEmailException(int userId)
        {
            // Given
            var user = new User
            {
                Id = _fixture.Context.Users.Count() + 1,
                Login = "new_userexistingemail",
                Name = "New UserExistingEmail",
                Email = _fixture.Context.Users.Find(userId).Email,
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = "NewUserExistingEmailCpf"
            };
            // When
            var act = async () => await _fixture.Service.CreateUser(user);
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(act);
            // Then
            Assert.True(aggregateException.InnerExceptions.Any(e => e is UsedEmailException));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        public async Task CreateUser_WithExistingCpf_ThrowsUsedCpfException(int userId)
        {
            // Given
            var user = new User
            {
                Id = _fixture.Context.Users.Count() + 1,
                Login = "new_userexistingcpf",
                Name = "New UserExistingCpf",
                Email = "newuserexistingcpf@MailAddress.com",
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = _fixture.Context.Users.Find(userId).Cpf
            };
            // When
            var act = async () => await _fixture.Service.CreateUser(user);
            var aggregateException = await Assert.ThrowsAsync<AggregateException>(act);
            // Then
            Assert.True(aggregateException.InnerExceptions.Any(e => e is UsedCpfException));
        }

        [Fact]
        public async Task GetAuthenticationToken_PassingRightCredentials_ReturnsTokenString()
        {
            // Given
            var authenticationRequest = new AuthenticationRequest()
            {
                Login = "admin",
                Password = "#tr1l0g0"
            };
            var mocker = new AutoMocker();
            mocker.GetMock<ITokenService>().Setup(u => u.GenerateToken(It.IsAny<User>())).Returns("token-service");
            var unitOfWork = new UnitOfWork(_fixture.Context);
            var userService = new UserService(unitOfWork, mocker.Get<ITokenService>());

            // When
            string tokenString = await userService.GetAuthenticationToken(authenticationRequest);

            // Then 
            Assert.IsType<string>(tokenString);
        }

        [Fact]
        public async Task GetAuthenticationToken_PassingWrongPasswordOfExistingUser_ThrowsUnauthorizedAccessException()
        {
            // Given
            var authenticationRequest = new AuthenticationRequest()
            {
                Login = "admin",
                Password = "wrongpassword"
            };

            // When
            var act = async () => await _fixture.Service.GetAuthenticationToken(authenticationRequest);

            // Then 
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Fact]
        public async Task GetAuthenticationToken_PassingUnexistingUser_ThrowsUnauthorizedAccessException()
        {
            // Given
            var authenticationRequest = new AuthenticationRequest()
            {
                Login = "notexistinguser",
                Password = "password"
            };

            // When
            var act = async () => await _fixture.Service.GetAuthenticationToken(authenticationRequest);

            // Then 
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Fact]
        public async Task DeleteUser_WhenCalled_DeleteUserFromDb()
        {
            // Given
            var userId = _fixture.Context.Users.Count() + 1;
            var userToDelete = new User()
            {
                Id = userId,
                Login = "deleting_user",
                Name = "Deleting User",
                Email = "deleting_user@mail.com",
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = "DeletingUserCpf"
            };
            _fixture.Context.Users.Add(userToDelete);
            _fixture.Context.SaveChanges();
            // When
            await _fixture.Service.DeleteUser(userToDelete);
            // Then
            Assert.True(!_fixture.Context.Users.Any(u => u.Id == userId));
        }

        [Fact]
        public async Task DeleteUser_WhenNullUserIsPassed_ThrowsRegisterNotFoundException()
        {
            // Given
            User userToDelete = null;
            // When
            var act = async () => await _fixture.Service.DeleteUser(userToDelete);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task DeleteUser_WhenUserHasRegisteredOrders_ThrowsNotAllowedDeletionException(int orderId)
        {
            // Given
            User userToDelete = _fixture.Context.Orders.Find(orderId).Client;
            // When
            var act = async () => await _fixture.Service.DeleteUser(userToDelete);
            // Then
            await Assert.ThrowsAsync<NotAllowedDeletionException>(act);
        }

        [Fact]
        public async Task DeleteUserById_WhenCalled_DeleteUserWithSpecifiedIdFromDb()
        {
            // Given
            var userId = _fixture.Context.Users.Count() + 1;
            var userToDelete = new User()
            {
                Id = userId,
                Login = "deleting_userbyid",
                Name = "Deleting UserById",
                Email = "deleting_userbyid@mail.com",
                Birthday = DateTime.Now,
                Password = "1234",
                Cpf = "DeletingUserByIdCpf"
            };
            _fixture.Context.Users.Add(userToDelete);
            _fixture.Context.SaveChanges();
            // When
            await _fixture.Service.DeleteUserById(userToDelete.Id);
            // Then
            Assert.True(!_fixture.Context.Users.Any(u => u.Id == userId));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task UpdateUser_WhenUpdateRequestIsPassed_UpdateFieldsInDb(int userId)
        {
            // Given
            User user = _fixture.Context.Users.Find(userId);
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Name = "UserUpdated",
                Password = "PasswordUpdated",
                Birthday = new DateTime(1900, 1, 1),
                Address = new Address()
                {
                    Number = "NumberUpdated",
                    Street = "StreetUpdated",
                    Neighborhood = "NeighbourhoodUpdated",
                    City = "CityUpdated",
                    State = "StateUpdate"
                }
            };
            // When
            User updatedUser = await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            Assert.Equal<User>(_fixture.Context.Users.Find(userId), updatedUser);
        }

        [Fact]
        public async Task UpdateUser_WhenNullUserIsPassed_ThrowsRegisterNotFoundException()
        {
            // Given
            User user = null;
            UpdateUserRequest updateRequest = new UpdateUserRequest();
            // When
            var act = async () => await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateUser_WhenOnlyNameIsPassed_UpdateUserInDbOnlyInName(int userId)
        {
            // Given
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, User>();
            }).CreateMapper();
            User user = _fixture.Context.Users.Find(userId);
            User userBeforeUpdate = mapper.Map<User>(user);
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Name = "UserUpdatedNameOnly",
            };
            // When
            User updatedUser = await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            Assert.True(userBeforeUpdate.Name != updatedUser.Name &&
                        userBeforeUpdate.Login == updatedUser.Login &&
                        userBeforeUpdate.Cpf == updatedUser.Cpf &&
                        userBeforeUpdate.Email == updatedUser.Email &&
                        userBeforeUpdate.Birthday == updatedUser.Birthday &&
                        userBeforeUpdate.CreationDate == updatedUser.CreationDate &&
                        userBeforeUpdate.Password == updatedUser.Password &&
                        userBeforeUpdate.Address == updatedUser.Address &&
                        userBeforeUpdate.Role == updatedUser.Role
                        );
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateUser_WhenOnlyBirthdayIsPassed_UpdateUserInDbOnlyInBirthday(int userId)
        {
            // Given
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, User>();
            }).CreateMapper();
            User user = _fixture.Context.Users.Find(userId);
            User userBeforeUpdate = mapper.Map<User>(user);
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Birthday = DateTime.Now,
            };
            // When
            User updatedUser = await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            Assert.True(userBeforeUpdate.Name == updatedUser.Name &&
                        userBeforeUpdate.Login == updatedUser.Login &&
                        userBeforeUpdate.Cpf == updatedUser.Cpf &&
                        userBeforeUpdate.Email == updatedUser.Email &&
                        userBeforeUpdate.Birthday != updatedUser.Birthday &&
                        userBeforeUpdate.CreationDate == updatedUser.CreationDate &&
                        userBeforeUpdate.Password == updatedUser.Password &&
                        userBeforeUpdate.Address == updatedUser.Address &&
                        userBeforeUpdate.Role == updatedUser.Role
                        );
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateUser_WhenOnlyPasswordIsPassed_UpdateUserInDbOnlyInPassword(int userId)
        {
            // Given
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, User>();
            }).CreateMapper();
            User user = _fixture.Context.Users.Find(userId);
            User userBeforeUpdate = mapper.Map<User>(user);
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Password = "UserUpdatedPasswordOnly",
            };
            // When
            User updatedUser = await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            Assert.True(userBeforeUpdate.Name == updatedUser.Name &&
                        userBeforeUpdate.Login == updatedUser.Login &&
                        userBeforeUpdate.Cpf == updatedUser.Cpf &&
                        userBeforeUpdate.Email == updatedUser.Email &&
                        userBeforeUpdate.Birthday == updatedUser.Birthday &&
                        userBeforeUpdate.CreationDate == updatedUser.CreationDate &&
                        userBeforeUpdate.Password != updatedUser.Password &&
                        userBeforeUpdate.Address == updatedUser.Address &&
                        userBeforeUpdate.Role == updatedUser.Role
                        );
        }

        [Theory]
        [InlineData(10)]
        public async Task UpdateUser_WhenUserHasNullAddress_CreateANewRegisterOfAddressInDb(int userId)
        {
            // Given
            User user = _fixture.Context.Users.Find(userId);
            user.Address = null;
            user.AddressId = null;
            UpdateUserRequest updateRequest = new UpdateUserRequest()
            {
                Address = new Address()
                {
                    Number = "NumberUpdated",
                    Street = "StreetUpdated",
                    Neighborhood = "NeighbourhoodUpdated",
                    City = "CityUpdated",
                    State = "StateUpdate"
                }
            };
            // When
            User updatedUser = await _fixture.Service.UpdateUser(user, updateRequest);
            // Then
            Assert.Equal<Address>(updatedUser.Address, _fixture.Context.Users.Find(userId).Address
                        );
        }
    }
}