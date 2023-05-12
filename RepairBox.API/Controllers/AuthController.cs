using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserServiceRepo _userRepo;
        public AuthController(IUserServiceRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            try
            {
                var message = _userRepo.Test();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}