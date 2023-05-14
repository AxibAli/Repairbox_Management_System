using RepairBox.BL.DTOs.Priority;
using RepairBox.Common.Commons;
using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IPriorityServiceRepo
    {
        List<GetPriorityDTO>? GetPriorities(); 
    }

    public class PriorityServiceRepo : IPriorityServiceRepo
    {
        public ApplicationDBContext _context;
        private int pageSize = DeveloperConstants.PAGE_SIZE;
        public PriorityServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public List<GetPriorityDTO>? GetPriorities()
        {
            List<GetPriorityDTO> priorities = new List<GetPriorityDTO>();
            var priorityList = _context.RepairPriorities.ToList();
            if (priorityList != null)
            {
                priorityList.ForEach(priority => priorities.Add(Omu.ValueInjecter.Mapper.Map<GetPriorityDTO>(priority)));
                return priorities;
            }
            return null;
        }
    }
}
