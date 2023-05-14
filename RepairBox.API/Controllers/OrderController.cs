using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.Order;
using RepairBox.BL.ServiceModels.Order;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;
using Stripe;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IPriorityServiceRepo _priorityRepo;
        private readonly IStripeService _stripeRepo;
        private readonly IOrderServiceRepo _orderRepo;

        public OrderController(IPriorityServiceRepo priorityRepo, IStripeService stripeRepo, IOrderServiceRepo orderRepo)
        {
            _priorityRepo = priorityRepo;
            _stripeRepo = stripeRepo;
            _orderRepo = orderRepo;
        }

        [HttpPost("GetPriorities")]
        public IActionResult GetPriorities()
        {
            try
            {
                var data = _priorityRepo.GetPriorities();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
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


        [HttpPost("CalculateOrderPrice")]
        public IActionResult CalculateOrderPrice(GetOrderChargesDTO model)
        {
            try
            {
                var data = _orderRepo.CalculateOrder(model);

                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }


        [HttpPost("StripePayment")]
        public async Task<IActionResult> StripePayment()
        {
            try
            {
                var message = await _stripeRepo.CreateCharge();

                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
