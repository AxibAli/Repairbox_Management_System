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

        [HttpGet("Login")]
        public IActionResult Login()
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

        [HttpGet("/Payment")]
        public IActionResult Payment()
        {
            try
            {
                var message = _userRepo.Test();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpGet("/Order")]
        public IActionResult Order()
        {
            try
            {
                var message = _userRepo.Test();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}