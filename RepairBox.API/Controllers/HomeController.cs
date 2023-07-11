using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IDashboardServiceRepo _dashboardRepo;
        public HomeController(IDashboardServiceRepo dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }
        [HttpGet("GetDashboard")]
        public IActionResult GetDashboard()
        {
            try
            {
                var data = _dashboardRepo.GetDashboardData();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
