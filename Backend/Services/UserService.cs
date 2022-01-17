using System;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using System.Collections.Generic;
using Backend.Data;
using Backend.Models;
using Backend.Models.ViewModels;
using Backend.Interfaces.Services;
using Backend.Utils;
using Backend.Models.Exceptions;
using Backend.Models.Enums;
using Backend.Interfaces.UnitOfWork;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public UserService(IUnitOfWork unitOfWork, ITokenService tService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tService;
        }

        public User GetUserById(User requestingUser, int id)
        {
            try
            {
                User user = _unitOfWork.UserRepository.GetBy(user => user.Id == id, "Address");
                if (user == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
                if (requestingUser.Role != "Administrator" && requestingUser.Login != user.Login)
                    throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.NotAuthorized));
                return user;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                return _unitOfWork.UserRepository.GetBy(user => user.Login == login);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public List<User> Query(string filter, string sort, int? queryPage)
        {
            try
            {
                Expression<Func<User, bool>> predicateFilter = null;
                Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null;
                Func<IQueryable<User>, IIncludableQueryable<User, object>> includes =
                user => user.Include(user => user.Address);

                if (!string.IsNullOrEmpty(filter))
                    predicateFilter = user => user.Login.Contains(filter) ||
                   user.Name.Contains(filter) ||
                   user.Email.Contains(filter);
                if (sort == "asc")
                    orderBy = q => q.OrderBy(user => user.Name);
                else if (sort == "desc")
                    orderBy = q => q.OrderByDescending(user => user.Name);
                return _unitOfWork.UserRepository.Get(predicateFilter, orderBy, includes, queryPage).ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public User CreateUser(User user)
        {
            user.Password = BC.HashPassword(user.Password);
            try
            {
                var exceptions = new List<Exception>();
                if (_unitOfWork.UserRepository.GetBy(u => u.Login == user.Login) != null)
                    exceptions.Add(new UsedLoginException(ErrorUtils.GetMessage(ErrorType.UniqueUserName)));
                if (_unitOfWork.UserRepository.GetBy(u => u.Email == user.Email) != null)
                    exceptions.Add(new UsedEmailException(ErrorUtils.GetMessage(ErrorType.UniqueUserEmail)));
                if (_unitOfWork.UserRepository.GetBy(u => u.Cpf == user.Cpf) != null)
                    exceptions.Add(new UsedCpfException(ErrorUtils.GetMessage(ErrorType.UniqueUserCpf)));
                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);
                _unitOfWork.UserRepository.Insert(user);
                _unitOfWork.Commit();
                return user;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public string GetAuthenticationToken(AuthUser authUser)
        {
            try
            {
                User user = GetUserByLogin(authUser.Login);
                if (user == null)
                    throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.IncorrectLoginOrPassword));
                if (!BC.Verify(authUser.Password, user.Password))
                    throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.IncorrectLoginOrPassword));
                return _tokenService.GenerateToken(user);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                User user = _unitOfWork.UserRepository.GetBy(user => user.Id == id);
                if (user == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.UserIdNotFound));
                if (_unitOfWork.OrderRepository.GetBy(order => order.Client.Id == id) != null)
                    throw new NotAllowedDeletionException(ErrorUtils.GetMessage(ErrorType.DeleteUserWithRegisteredOrder));
                else
                {
                    _unitOfWork.UserRepository.Delete(id);
                    _unitOfWork.Commit();
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (NotAllowedDeletionException)
            {
                throw;
            }
        }

        public User UpdateUser(int id, UpdateUser userUpdate)
        {
            try
            {
                User user = _unitOfWork.UserRepository.GetBy(user => user.Id == id, "Address");
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
                _unitOfWork.Commit();
                return user;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}