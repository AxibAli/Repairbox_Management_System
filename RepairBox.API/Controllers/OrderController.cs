using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.ServiceModels.Order;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public OrderController()
        {

        }

        [HttpPost("CreateBooking")]
        public IActionResult CreateOrder(AddOrderDTO addOrderDTO)
        {
            try
            {
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
