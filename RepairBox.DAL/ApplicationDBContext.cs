using Microsoft.EntityFrameworkCore;
using RepairBox.DAL.Entities;

namespace RepairBox.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> opt) : base(opt)
        {

        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDefect> OrderDefects => Set<OrderDefect>();
        public DbSet<RepairPriority> RepairPriorities => Set<RepairPriority>();
        public DbSet<RepairStatus> RepairStatuses => Set<RepairStatus>();
        public DbSet<Setting> Settings => Set<Setting>();
        public DbSet<Model> Models => Set<Model>();
        public DbSet<RepairableDefect> RepairableDefects => Set<RepairableDefect>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<CustomerInfo> CustomerInfos => Set<CustomerInfo>();
        public DbSet<CustomerIdentities> CustomerIdentities => Set<CustomerIdentities>();
        public DbSet<DeviceInfo> DeviceInfos => Set<DeviceInfo>();
        public DbSet<PurchaseFromCustomerInvoice> PurchaseFromCustomerInvoices => Set<PurchaseFromCustomerInvoice>();
        public DbSet<Company> Companies => Set<Company>();
    }
}