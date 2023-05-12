using Microsoft.EntityFrameworkCore;

namespace RepairBox.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> opt) : base(opt)
        {

        }
    }
}