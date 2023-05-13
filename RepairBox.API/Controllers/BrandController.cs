using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandServiceRepo _brandRepo;
        public BrandController(IBrandServiceRepo brandRepo)
        {
            _brandRepo = brandRepo;
        }

        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand(string Name)
        {
            try
            {
                await _brandRepo.AddBrand(Name);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("UpdateBrand")]
        public async Task<IActionResult> UpdateBrand(int Id, string Name)
        {
            try
            {
                await _brandRepo.UpdateBrand(Id, Name);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpPost("DeleteBrand")]
        public async Task<IActionResult> DeleteBrand(int Id)
        {
            try
            {
                await _brandRepo.DeleteBrand(Id);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetBrand")]
        public IActionResult GetBrand(int brandId)
        {
            try
            {
                var data = _brandRepo.GetBrand(brandId);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetBrands")]
        public IActionResult GetBrands(string? query, int pageNo = 1)
        {
            try
            {
                var data = _brandRepo.GetBrands(query, pageNo);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
