using Microsoft.AspNetCore.Http;
using RepairBox.BL.DTOs.Dashboard;
using RepairBox.BL.DTOs.User;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IDashboardServiceRepo
    {
        GetDashboardDTO GetDashboardData();
    }
    public class DashboardServiceRepo : IDashboardServiceRepo
    {
        private ApplicationDBContext _context;
        public DashboardServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public GetDashboardDTO GetDashboardData()
        {


            int brandsCount = _context.Brands.Select(brand => brand.Id).Count();
            int deviceCount = _context.Models.Select(model => model.Id).Count();
            int defectsCount = _context.RepairableDefects.Select(defect => defect.Id).Count();

            var now = DateTime.Now;
            var twelveMonthsAgo = now.AddMonths(-12);

            var annualRepairOrders = _context.Orders
                    .Where(order => order.CreatedAt >= twelveMonthsAgo)
                    .GroupBy(order => new { order.CreatedAt.Year, order.CreatedAt.Month })
                    .Select(group => new { group.Key.Year, group.Key.Month, order_count = group.Count() })
                    .OrderBy(group => group.Year)
                    .ThenBy(group => group.Month)
                    .ToList();

            var annualRepairOrdersList = new List<AnnualRepairOrder>();

            foreach (var order in annualRepairOrders)
            {
                annualRepairOrdersList.Add(new AnnualRepairOrder
                {
                    Year = order.Year,
                    Month = order.Month,
                    OrderCount = order.order_count
                });
            }

            var dashboard = new GetDashboardDTO
            {

                Brands = brandsCount,
                Devices = deviceCount,
                Defects = defectsCount,
                AnnualRepairOrders = annualRepairOrdersList
            };

            return dashboard;
        }
    }
}
