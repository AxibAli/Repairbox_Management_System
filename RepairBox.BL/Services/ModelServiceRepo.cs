﻿using Microsoft.EntityFrameworkCore;
using RepairBox.BL.DTOs.Model;
using RepairBox.Common.Commons;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
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
        Task AddModels(List<AddModelDTO> datum, int brandId);
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

        public async Task AddModels(List<AddModelDTO> datum, int brandId)
        {
            List<Model> models = new List<Model>();
            foreach (var data in datum)
            {
                models.Add(new Model
                {
                    Name = data.Name,
                    ModelName = data.Model,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    BrandId = brandId
                });
            }
            var modelNames = models.Select(x => x.ModelName).ToList();
            int index = 0;
            foreach (var modelName in modelNames)
            {
                var d = IGetModels().FirstOrDefault(d => d.ModelName.Contains(modelName));
                if (d != null)
                {
                    models.RemoveAt(index);
                    index = 0;
                }
                else
                    index++;
            }

            if (models.Count > 0)
            {
                await _context.Models.AddRangeAsync(models);
                await _context.SaveChangesAsync();
            }
        }

        private IQueryable<Model> IGetModels()
        {
            return _context.Models.AsQueryable();
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
                         select new GetModelDTO
                         {
                             Id = m.Id,
                             Name = m.Name,
                             ModelName = m.ModelName,
                             BrandId = m.BrandId
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
            var modelQuery = _context.Models.AsQueryable();
            var models = modelQuery.Where(b => query != null ? b.Name.StartsWith(query) : true).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            models.ForEach(model => modelList.Add(Omu.ValueInjecter.Mapper.Map<GetModelDTO>(model)));

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
            var modelQuery = _context.Models.AsQueryable();
            var models = modelQuery.Where(b => query != null ? b.Name.StartsWith(query) : true && b.Id == brandId).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            models.ForEach(model => modelList.Add(Omu.ValueInjecter.Mapper.Map<GetModelDTO>(model)));

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
