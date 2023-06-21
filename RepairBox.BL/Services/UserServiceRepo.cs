using RepairBox.BL.DTOs.User;
using RepairBox.Common.Commons;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IUserServiceRepo
    {
        bool VerifyUserLogin(UserLoginDTO userLoginDTO);
        bool CreateUser(CreateUserDTO userDTO);
    }
    public class UserServiceRepo : IUserServiceRepo
    {
        public ApplicationDBContext _context;
        public UserServiceRepo(ApplicationDBContext context)
        {
            _context = context;
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
                IsActive = userDTO.Status,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Add reference in User Role

            return true;
        }
    }
}
