using Microsoft.EntityFrameworkCore;
using RepairBox.BL.DTOs.Model;
using RepairBox.Common.Commons;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RepairBox.BL.Services
{
    public interface IModelServiceRepo
    {
        Task AddModels(List<AddModelDTO> datum);
        Task DeleteModel(int brandId);
        Task UpdateModel(UpdateModelDTO data);
        IQueryable<SelectListItem> GetModels();
        GetModelDTO? GetModel(int modelId);
        PaginationModel GetModels(string query, int pageNo);
        PaginationModel GetModelsByBrandId(string query, int pageNo, int brandId);
    }
    public class ModelServiceRepo : IModelServiceRepo
    {
        public ApplicationDBContext _context;
        private int pageSize = DeveloperConstants.PAGE_SIZE;
        public ModelServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public Task AddModels(List<AddModelDTO> datum)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteModel(int modelId)
        {
            var model = _context.Models.FirstOrDefault(m => m.Id == modelId);
            model.IsDeleted = false;
            await _context.SaveChangesAsync();
        }

        public GetModelDTO? GetModel(int modelId)
        {
            var model = (from m in _context.Models
                     join b in _context.Brands
                     on m.BrandId equals b.Id
                     select new GetModelDTO
                     {
                         Id = m.Id,
                         Name = m.Name,
                         ModelName = m.ModelName,
                         Brand = b.Name
                     }).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return null;
        }

        public IQueryable<SelectListItem> GetModels()
        {
            var models = _context.Models.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            });

            return models;
        }

        public PaginationModel GetModels(string query, int pageNo)
        {
            List<GetModelDTO> modelList = new List<GetModelDTO>();
            var modelQuery = _context.Brands.AsQueryable();
            var models = modelQuery.Where(b => query != null ? b.Name.StartsWith(query) : true).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            models.ForEach(brand => modelList.Add(Omu.ValueInjecter.Mapper.Map<GetModelDTO>(brand)));

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(modelQuery.Count(), pageSize),
                CurrentPage = pageNo,
                Data = modelList
            };
        }
        
        public PaginationModel GetModelsByBrandId(string query, int pageNo, int brandId)
        {
            List<GetModelDTO> modelList = new List<GetModelDTO>();
            var modelQuery = _context.Brands.AsQueryable();
            var models = modelQuery.Where(b => query != null ? b.Name.StartsWith(query) : true && b.Id == brandId).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            models.ForEach(brand => modelList.Add(Omu.ValueInjecter.Mapper.Map<GetModelDTO>(brand)));

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(modelQuery.Count(), pageSize),
                CurrentPage = pageNo,
                Data = modelList
            };
        }

        public async Task UpdateModel(UpdateModelDTO data)
        {
            var model = _context.Models.FirstOrDefault(m => m.Id == data.Id);
            model.Name = data.Name;
            model.ModelName = data.ModelName;
            await _context.SaveChangesAsync();
        }
    }
}
