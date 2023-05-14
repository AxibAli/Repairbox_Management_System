using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.Model;
using RepairBox.BL.DTOs.RepairDefect;
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
        private IRepairDefectServiceRepo _defectRepo;
        public BrandController(IBrandServiceRepo brandRepo, IModelServiceRepo modelRepo, IRepairDefectServiceRepo defectRepo)
        {
            _brandRepo = brandRepo;
            _modelRepo = modelRepo;
            _defectRepo = defectRepo;
        }

        #region Brand
        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand(string Name)
        {
            try
            {
                await _brandRepo.AddBrand(Name);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.ADDED_SUCCESSFULLY, "Brand") });
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
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.UPDATED_SUCCESSFULLY, "Brand") });
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
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.DELETED_SUCCESSFULLY, "Brand") });
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
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.ADDED_SUCCESSFULLY, "Model") });
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
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.UPDATED_SUCCESSFULLY, "Model") });
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
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.DELETED_SUCCESSFULLY, "Model") });
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
        
        #region Defect

        [HttpPost("AddDefects")]
        public async Task<IActionResult> AddDefects()
        {
            try
            {
                List<AddDefectDTO> models = new List<AddDefectDTO>();
                await _defectRepo.AddDefects(models);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.ADDED_SUCCESSFULLY, "Defect") });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        [HttpPost("UpdateDefect")]
        public async Task<IActionResult> UpdateDefect(UpdateDefectDTO model)
        {
            try
            {
                await _defectRepo.UpdateDefect(model);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.UPDATED_SUCCESSFULLY, "Defect") });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpPost("DeleteDefect")]
        public async Task<IActionResult> DeleteDefect(int Id)
        {
            try
            {
                await _defectRepo.DeleteDefect(Id);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.DELETED_SUCCESSFULLY, "Defect") });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetDefect")]
        public IActionResult GetDefect(int defectId)
        {
            try
            {
                var data = _defectRepo.DeleteDefect(defectId);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpGet("GetDefects")]
        public IActionResult GetDefects(string? query, int pageNo = 1)
        {
            try
            {
                var data = _defectRepo.GetDefects(query, pageNo);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpGet("GetModelDefects")]
        public IActionResult GetModelDefects(string? query,int modelId, int pageNo = 1)
        {
            try
            {
                var data = _defectRepo.GetDefectsByModelId(query, pageNo, modelId);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
        
        [HttpGet("GetDefectsforDropdown")]
        public IActionResult GetDefectsforDropdown()
        {
            try
            {
                var data = _defectRepo.GetDefects();
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
