using RepairBox.BL.ServiceModels.Brand;
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
    public interface IBrandServiceRepo
    {
        Task AddBrand(string Name);
        Task UpdateBrand(int brandId, string Name);
        Task DeleteBrand(int brandId);
        GetBrandDTO? GetBrand(int brandId);
        PaginationModel GetBrands(string query, int pageNo);
        IQueryable<SelectListItem> GetBrands();
    }
    public class BrandServiceRepo : IBrandServiceRepo
    {
        public ApplicationDBContext _context;
        private int pageSize = DeveloperConstants.PAGE_SIZE;

        public BrandServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddBrand(string Name)
        {
            await _context.Brands.AddAsync(new DAL.Entities.Brand
            {
                Name = Name,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            }); 

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrand(int brandId)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == brandId);
            brand.IsDeleted = false;
            await _context.SaveChangesAsync();
        }

        public GetBrandDTO? GetBrand(int brandId)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == brandId);
            if (brand != null)
                return Omu.ValueInjecter.Mapper.Map<GetBrandDTO>(brand);
            return null; 
        }

        public PaginationModel GetBrands(string query, int pageNo = 1)
        {
            List<GetBrandDTO> brandList = new List<GetBrandDTO>();
            var brandQuery = _context.Brands.AsQueryable();
            var brands = brandQuery.Where(b => query != null ? b.Name.StartsWith(query) : true).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
            brands.ForEach(brand => brandList.Add(Omu.ValueInjecter.Mapper.Map<GetBrandDTO>(brand)));

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(brandQuery.Count(), pageSize),
                CurrentPage = pageNo,
                Data = brandList
            };
        }

        public IQueryable<SelectListItem> GetBrands()
        {
            var brands = _context.Brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            });

            return brands;
        }

        public async Task UpdateBrand(int brandId, string Name)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.Id == brandId);
            brand.Name = Name;
            await _context.SaveChangesAsync();
        }
    }
}
