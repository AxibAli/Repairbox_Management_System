﻿using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IUserServiceRepo
    {
        string Test();
    }
    public class UserServiceRepo : IUserServiceRepo
    {
        public ApplicationDBContext _context;
        public UserServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public string Test()
        {
            return "Testing";
        }
    }
}
