using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.DAL.Entities
{
    public class CustomerIdentities : Base
    {
        [ForeignKey("CustomerInfo")]
        public int CustomerInfoId { get; set; }
        public byte[] Image1 { get; set; }
        public byte[] Image2 { get; set; }
        public virtual CustomerInfo CustomerInfo { get; set; }
    }
}
