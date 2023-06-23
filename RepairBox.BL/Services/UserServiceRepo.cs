﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RepairBox.BL.DTOs.Permission;
using RepairBox.BL.DTOs.Role;
using RepairBox.BL.DTOs.User;
using RepairBox.Common.Commons;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IUserServiceRepo
    {
        bool VerifyUserLogin(UserLoginDTO userLoginDTO);
        List<GetUserDTO?> GetUsers();
        GetUserDTO? GetUserById(int id);
        bool CreateUser(CreateUserDTO userDTO);
        void ModifySelfUser(UpdateSelfUserDTO userDTO);
        void ModifyOtherUser(UpdateOtherUserDTO userDTO);
        bool DeleteUser(int id);
        (string, bool) ChangePassword(UserChangePasswordDTO changePasswordDTO);
        string GetMyEmail();
        void SetRefreshToken(string email, RefreshToken refreshToken);
        RefreshToken GetRefreshToken(string email);
        void ClearRefreshToken(string userEmail);
    }
    public class UserServiceRepo : IUserServiceRepo
    {
        public ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserServiceRepo(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<GetUserDTO?> GetUsers()
        {
            List<GetUserDTO> users = new List<GetUserDTO>();
            var userList = _context.Users.ToList();

            if (userList == null) { return null; }

            userList.ForEach(user => users.Add(Omu.ValueInjecter.Mapper.Map<GetUserDTO>(user)));

            return users;
        }

        public GetUserDTO? GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null) { return null; }

            var userDTO = Omu.ValueInjecter.Mapper.Map<GetUserDTO>(user);

            return userDTO;
        }

        public bool VerifyUserLogin(UserLoginDTO userLoginDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == userLoginDTO.Email);
            if (user == null)
            {
                return false;
            }
            else
            {
                return CommonHelper.VerifyPassword(userLoginDTO.Password, user.PasswordHash, user.PasswordSalt);
            }
        }

        public bool CreateUser(CreateUserDTO userDTO)
        {
            var checkUserEmail = _context.Users.FirstOrDefault(u => u.Email == userDTO.Email);
            if(checkUserEmail != null) { return false; }

            (string hash, string salt) = CommonHelper.GenerateHashAndSalt(userDTO.Password);

            var user = new User
            {
                Username = userDTO.Username,
                Email = userDTO.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserRoleId = userDTO.UserRoleId,
                IsActive = userDTO.Status,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        public void ModifySelfUser(UpdateSelfUserDTO userDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userDTO.Id);
            if (user != null)
            {
                user.Username = userDTO.Username;
                user.Email = userDTO.Email;

                _context.SaveChanges();
            }

            return;
        }

        public void ModifyOtherUser(UpdateOtherUserDTO userDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userDTO.Id);
            if (user != null)
            {
                user.Username = userDTO.Username;
                user.Email = userDTO.Email;
                user.UserRoleId = userDTO.UserRoleId;
                user.IsActive = userDTO.Status;

                _context.SaveChanges();
            }
        }

        public bool DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            
            if (user == null) { return false; }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return true;
        }

        public (string, bool) ChangePassword(UserChangePasswordDTO changePasswordDTO)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == changePasswordDTO.Id);

            if(user == null)
            {
                return (CustomMessage.USER_NOT_EXIST, false);
            }

            var verify = CommonHelper.VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash, user.PasswordSalt);

            if(!verify)
            {
                return (CustomMessage.PASSWORD_INCORRECT, false);
            }

            if (changePasswordDTO.NewPassword1 != changePasswordDTO.NewPassword2)
            {
                return (CustomMessage.PASSWORD_NOT_MATCH, false);
            }
            else
            {
                (string hash, string salt) = CommonHelper.GenerateHashAndSalt(changePasswordDTO.NewPassword1);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;

                return (CustomMessage.PASSWORD_CHANGED, true);
            }
        }

        public string GetMyEmail()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                var emailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim != null)
                {
                    result = emailClaim.Value;
                }
            }
            return result;
        }

        public void SetRefreshToken(string email, RefreshToken refreshToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;
        }

        public RefreshToken? GetRefreshToken(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if(user == null) { return null; }

            var refreshToken = new RefreshToken
            {
                Token = user.RefreshToken,
                Created = user.TokenCreated,
                Expires = user.TokenExpires
            };

            return refreshToken;
        }
        
        public void ClearRefreshToken(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.RefreshToken = "";
                _context.SaveChanges();
            }
        }
    }
}
