using RepairBox.BL.DTOs.Order;
using RepairBox.Common.Commons;
using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IOrderServiceRepo
    {
        CalculateOrderAmountDTO CalculateOrder(GetOrderChargesDTO model);
    }

    public class OrderServiceRepo : IOrderServiceRepo
    {
        public ApplicationDBContext _context;
        private int pageSize = DeveloperConstants.PAGE_SIZE;
        public OrderServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public CalculateOrderAmountDTO CalculateOrder(GetOrderChargesDTO model)
        {
            decimal subTotal = 0;
            decimal tax = 0;
            var defects = _context.RepairableDefects.Where(d => model.Defects.Contains(d.Id)).ToList();
            defects.ForEach(d => subTotal += d.Price);

            var setting = _context.Settings.FirstOrDefault();
            if (setting != null)
                tax = setting.Tax;

            var priorityProcessCharges = _context.RepairPriorities.FirstOrDefault(p => p.Id == model.PriorityId)?.ProcessCharges;

            decimal totalAmount = subTotal + tax + priorityProcessCharges.Value;

            return new CalculateOrderAmountDTO
            {
                SubTotal = subTotal,
                Tax = tax,
                PriorityProcessCharges = priorityProcessCharges.Value,
                TotalAmount = totalAmount
            };
        }
    }
}
