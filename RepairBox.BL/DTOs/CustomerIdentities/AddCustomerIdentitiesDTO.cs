using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.DTOs.CustomerIdentities
{
    public class AddCustomerIdentitiesDTO
    {
        public int Id { get; set; }
        public int CustomerInfoId { get; set; }
        public string Image { get; set; }
    }
}
