using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.DeviceInfo;
using RepairBox.BL.DTOs.CustomerProductPurchase;
using RepairBox.BL.ServiceModels.Order;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class CustomerProductPurchaseController : ControllerBase
    {
        private ICustomerProductPurchaseServiceRepo _customerProductPurchaseRepo;
        public CustomerProductPurchaseController(ICustomerProductPurchaseServiceRepo customerProductPurchaseRepo)
        {
            _customerProductPurchaseRepo = customerProductPurchaseRepo;
        }

        [HttpPost("AddCustomerProductPurchase")]
        public IActionResult AddCustomerProductPurchase(AddCustomerProductPurchaseDTO addCustomerProductPurchase)
        {
            AddCustomerInfoDTO customerInfo = addCustomerProductPurchase.CustomerInfo;
            AddCustomerIdentitiesDTO customerIdentities = addCustomerProductPurchase.CustomerIdentities;
            AddDeviceInfoDTO deviceInfo = addCustomerProductPurchase.DeviceInfo;

            try
            {
                _customerProductPurchaseRepo.AddCustomerProductPurchase(customerInfo, customerIdentities, deviceInfo);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = String.Format(CustomMessage.ADDED_SUCCESSFULLY, "Customer Product Purchase") });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
