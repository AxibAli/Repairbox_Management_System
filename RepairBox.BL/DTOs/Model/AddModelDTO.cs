using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.DTOs.Model
{
    public class AddModelDTO
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
    }
}
