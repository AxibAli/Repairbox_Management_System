﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.Model;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private IBrandServiceRepo _brandRepo;
        private IModelServiceRepo _modelRepo;
        public BrandController(IBrandServiceRepo brandRepo, IModelServiceRepo modelRepo)
        {
            _brandRepo = brandRepo;
            _modelRepo = modelRepo;
        }

        #region Brand
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
        
        [HttpGet("GetBrandsforDropdown")]
        public IActionResult GetBrandsforDropdown()
        {
            try
            {
                var data = _brandRepo.GetBrands();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        #endregion

        #region Model

        [HttpPost("AddModels")]
        public async Task<IActionResult> AddModels()
        {
            try
            {
                List<AddModelDTO> models = new List<AddModelDTO>();
                await _modelRepo.AddModels(models);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        [HttpPost("UpdateModel")]
        public async Task<IActionResult> UpdateModel(int Id, string Name)
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
        
        [HttpPost("DeleteModel")]
        public async Task<IActionResult> DeleteModel(int Id)
        {
            try
            {
                await _modelRepo.DeleteModel(Id);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "" });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetModel")]
        public IActionResult GetModel(int modelId)
        {
            try
            {
                var data = _modelRepo.GetModel(modelId);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetModels")]
        public IActionResult GetModels(string? query, int pageNo = 1)
        {
            try
            {
                var data = _modelRepo.GetModels(query, pageNo);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpGet("GetBrandModels")]
        public IActionResult GetBrandModels(string? query,int brandId, int pageNo = 1)
        {
            try
            {
                var data = _modelRepo.GetModelsByBrandId(query, pageNo, brandId);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpGet("GetModelsforDropdown")]
        public IActionResult GetModelsforDropdown()
        {
            try
            {
                var data = _modelRepo.GetModels();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        #endregion

    }
}
