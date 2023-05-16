using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.Order;
using RepairBox.BL.ServiceModels.Order;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;
using RestSharp;
using Stripe;
using System.Text;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IPriorityServiceRepo _priorityRepo;

        private readonly IOrderServiceRepo _orderRepo;

        public OrderController(IPriorityServiceRepo priorityRepo, IOrderServiceRepo orderRepo)
        {
            _priorityRepo = priorityRepo;
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

        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder(AddOrderDTO model)
        {
            try
            {
                _orderRepo.CreateOrder(model);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = String.Format(CustomMessage.ADDED_SUCCESSFULLY, "Order") });
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
    }
}
