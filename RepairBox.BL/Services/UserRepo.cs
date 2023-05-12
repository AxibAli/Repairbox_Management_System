using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IUserRepo
    {
        string Test();
    }
    public class UserRepo : IUserRepo
    {
        public UserRepo()
        {

        }

        public string Test()
        {
            return "Testing";
        }
    }
}
