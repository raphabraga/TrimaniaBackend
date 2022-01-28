using System;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using System.Collections.Generic;
using Backend.Models;
using Backend.Interfaces.Services;
using Backend.Utils;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Interfaces.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Backend.Dtos;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public UserService() { }

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public UserService(IUnitOfWork unitOfWork, ITokenService tService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tService;
        }

        public async Task<User> GetUserById(User requestingUser, int id)
        {
            User user = await _unitOfWork.UserRepository.GetBy(user => user.Id == id, "Address");
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            if (requestingUser.Role != "Administrator" && requestingUser.Id != user.Id)
                throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.NotAuthorized));
            return user;
        }

        public async Task<User> GetUserByLogin(string login)
        {
            var user = await _unitOfWork.UserRepository.GetBy(user => user.Login == login, "Address");
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CredentialsNotFound));
            return user;
        }

        public async Task<List<User>> GetUsers(SearchUserRequest searchRequest)
        {
            Expression<Func<User, bool>> predicateFilter = null;
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null;
            Func<IQueryable<User>, IIncludableQueryable<User, object>> includes =
            user => user.Include(user => user.Address);

            if (!string.IsNullOrEmpty(searchRequest.Filter))
                predicateFilter = user => user.Login.Contains(searchRequest.Filter) ||
               user.Name.Contains(searchRequest.Filter) ||
               user.Email.Contains(searchRequest.Filter);
            if (searchRequest.SortBy == "asc")
                orderBy = q => q.OrderBy(user => user.Name);
            else if (searchRequest.SortBy == "desc")
                orderBy = q => q.OrderByDescending(user => user.Name);
            var users = await _unitOfWork.UserRepository.Get(predicateFilter, orderBy, includes, searchRequest.Page);
            return users.ToList();
        }

        public async Task<User> CreateUser(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            var exceptions = new List<Exception>();
            if (await _unitOfWork.UserRepository.GetBy(u => u.Login == user.Login) != null)
                exceptions.Add(new UsedLoginException(ErrorUtils.GetMessage(ErrorType.UniqueUserName)));
            if (await _unitOfWork.UserRepository.GetBy(u => u.Email == user.Email) != null)
                exceptions.Add(new UsedEmailException(ErrorUtils.GetMessage(ErrorType.UniqueUserEmail)));
            if (await _unitOfWork.UserRepository.GetBy(u => u.Cpf == user.Cpf) != null)
                exceptions.Add(new UsedCpfException(ErrorUtils.GetMessage(ErrorType.UniqueUserCpf)));
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
            _unitOfWork.UserRepository.Insert(user);
            await _unitOfWork.Commit();
            return user;
        }

        public async Task<string> GetAuthenticationToken(AuthenticationRequest authUser)
        {
            User user;
            try
            {
                user = await GetUserByLogin(authUser.Login);
            }
            catch (RegisterNotFoundException)
            {
                throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.IncorrectLoginOrPassword));
            }
            if (!BC.Verify(authUser.Password, user.Password))
                throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.IncorrectLoginOrPassword));
            return _tokenService.GenerateToken(user);
        }

        public async Task DeleteUser(User user)
        {
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            if (await _unitOfWork.OrderRepository.GetBy(order => order.Client.Id == user.Id) != null)
                throw new NotAllowedDeletionException(ErrorUtils.GetMessage(ErrorType.DeleteUserWithRegisteredOrder));
            else
            {
                _unitOfWork.UserRepository.Delete(user.Id);
                await _unitOfWork.Commit();
            }
        }
        public async Task DeleteUserById(int id)
        {
            User user = await _unitOfWork.UserRepository.GetBy(user => user.Id == id);
            await DeleteUser(user);
        }

        public async Task<User> UpdateUser(User user, UpdateUserRequest userUpdate)
        {
            if (user == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
            user = PerformUpdate(user, userUpdate);
            await _unitOfWork.Commit();
            return user;
        }

        private User PerformUpdate(User user, UpdateUserRequest userUpdate)
        {
            user.Name = string.IsNullOrEmpty(userUpdate.Name) ? user.Name : userUpdate.Name;
            if (userUpdate.Password != null)
                user.Password = BC.HashPassword(userUpdate.Password);
            user.Birthday = (userUpdate.Birthday == null) ? user.Birthday : userUpdate.Birthday;
            if (user.Address == null)
            {
                if (userUpdate.Address != null)
                    user.Address = new Address
                    {
                        State = userUpdate?.Address?.State,
                        City = userUpdate?.Address?.City,
                        Neighborhood = userUpdate?.Address?.Neighborhood,
                        Street = userUpdate?.Address?.Street,
                        Number = userUpdate?.Address?.Number
                    };
            }
            else
            {
                if (userUpdate.Address != null)
                {
                    user.Address.State = (userUpdate?.Address?.State == null) ? user?.Address?.State : userUpdate?.Address?.State;
                    user.Address.City = (userUpdate?.Address?.City == null) ? user?.Address?.City : userUpdate?.Address?.City;
                    user.Address.Neighborhood = (userUpdate?.Address?.Neighborhood == null) ? user?.Address?.Neighborhood : userUpdate?.Address?.Neighborhood;
                    user.Address.Street = (userUpdate?.Address?.Street == null) ? user?.Address?.Street : userUpdate?.Address?.Street;
                    user.Address.Number = (userUpdate?.Address?.Number == null) ? user?.Address?.Number : userUpdate?.Address?.Number;
                }
            }
            return user;
        }
    }
}