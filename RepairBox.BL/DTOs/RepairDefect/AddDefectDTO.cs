using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.DTOs.RepairDefect
{
    public class AddDefectDTO
    {
        public string DefectName { get; set; }
        public string RepairTime { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int ModelId { get; set; }
    }
}
