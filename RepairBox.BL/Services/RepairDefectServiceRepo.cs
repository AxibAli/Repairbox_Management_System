﻿using RepairBox.BL.DTOs.RepairDefect;
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
    public interface IRepairDefectServiceRepo
    {
        Task AddDefects(List<AddDefectDTO> datum);
        Task DeleteDefect(int defectId);
        Task UpdateDefect(UpdateDefectDTO data);
        IQueryable<SelectListItem> GetDefects();
        GetDefectDTO? GetDefect(int defectId);
        PaginationModel GetDefects(string query, int pageNo);
        PaginationModel GetDefectsByModelId(string query, int pageNo, int modelId);
    }

    public class RepairDefectServiceRepo : IRepairDefectServiceRepo
    {
        public ApplicationDBContext _context;
        private int pageSize = DeveloperConstants.PAGE_SIZE;
        public RepairDefectServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public Task AddDefects(List<AddDefectDTO> datum)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteDefect(int defectId)
        {
            var defect = _context.RepairableDefects.FirstOrDefault(d => d.Id == defectId);
            defect.IsDeleted = false;
            await _context.SaveChangesAsync();
        }

        public GetDefectDTO? GetDefect(int defectId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<SelectListItem> GetDefects()
        {
            var models = _context.RepairableDefects.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.DefectName
            });

            return models;
        }

        public PaginationModel GetDefects(string query, int pageNo)
        {
            List<GetDefectDTO> defectList = new List<GetDefectDTO>();
            var defectQuery = _context.Brands.AsQueryable();
            var defects = defectQuery.Where(d => query != null ? d.Name.StartsWith(query) : true).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            defects.ForEach(brand => defectList.Add(Omu.ValueInjecter.Mapper.Map<GetDefectDTO>(brand)));

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(defectQuery.Count(), pageSize),
                CurrentPage = pageNo,
                Data = defectList
            };
        }

        public PaginationModel GetDefectsByModelId(string query, int pageNo, int modelId)
        {
            List<GetDefectDTO> defectList = new List<GetDefectDTO>();
            var defectQuery = _context.Brands.AsQueryable();
            var defects = defectQuery.Where(d => query != null ? d.Name.StartsWith(query) : true && d.Id == modelId).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            defects.ForEach(brand => defectList.Add(Omu.ValueInjecter.Mapper.Map<GetDefectDTO>(brand)));

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(defectQuery.Count(), pageSize),
                CurrentPage = pageNo,
                Data = defectList
            };
        }

        public async Task UpdateDefect(UpdateDefectDTO data)
        {
            var defect = _context.RepairableDefects.FirstOrDefault(d => d.Id == data.Id);
            defect.DefectName = data.DefectName;
            defect.RepairTime = data.RepairTime;
            defect.Cost = data.Cost;
            defect.Price = data.Price;
            await _context.SaveChangesAsync();
        }
    }
}