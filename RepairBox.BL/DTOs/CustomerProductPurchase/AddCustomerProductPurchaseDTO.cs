using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.DTOs.CustomerProductPurchase
{
    public class AddCustomerProductPurchaseDTO
    {
        public AddCustomerInfoDTO CustomerInfo { get; set; }
        public AddCustomerIdentitiesDTO CustomerIdentities { get; set; }
        public AddDeviceInfoDTO DeviceInfo { get; set; }
    }
}
