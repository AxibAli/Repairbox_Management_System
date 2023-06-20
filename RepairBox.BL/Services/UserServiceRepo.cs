using RepairBox.BL.DTOs.User;
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
        
    }
    public class UserServiceRepo : IUserServiceRepo
    {
        public ApplicationDBContext _context;
        public UserServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public bool VerifyUserLogin(UserLogin userLogin)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userLogin.Username);
            if (user == null)
            {
                return false;
            }
            else
            {
                return CommonHelper.VerifyPassword(userLogin.Password, user.PasswordHash, user.PasswordSalt);
            }
        }

        public void CreateUser(AddUserDTO userDTO)
        {
            (string hash, string salt) = CommonHelper.GenerateHashAndSalt(userDTO.Password); 

            var user = new User
            {
                Username = userDTO.Username,
                Email = userDTO.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
