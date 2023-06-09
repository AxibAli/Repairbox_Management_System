using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.DeviceInfo;
using RepairBox.BL.DTOs.Model;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RepairBox.BL.Services
{
    public interface ICustomerProductPurchaseServiceRepo
    {
        void AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO);
    }
    public class CustomerProductPurchaseServiceRepo : ICustomerProductPurchaseServiceRepo
    {
        public ApplicationDBContext _context;
        public CustomerProductPurchaseServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public void AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO)
        {
            var customerInfo = new CustomerInfo
            {
                Name = customerInfoDTO.Name,
                Email = customerInfoDTO.Email,
                PhoneNumber = customerInfoDTO.PhoneNumber,
                Address = customerInfoDTO.Address,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };

            _context.CustomerInfos.Add(customerInfo);
            _context.SaveChanges();

            var customerIdentities = new CustomerIdentities
            {
                CustomerInfoId = customerInfo.Id,
                Image = customerIdentitiesDTO.Image,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };

            _context.CustomerIdentities.Add(customerIdentities);
            _context.SaveChanges();

            var deviceInfo = new DeviceInfo
            {
                CustomerInfoId = customerInfo.Id,
                DeviceNameModel = deviceInfoDTO.DeviceNameModel,
                IMEI = deviceInfoDTO.IMEI,
                SerialNumber = deviceInfoDTO.SerialNumber,
                Cost = deviceInfoDTO.Cost,
                Price = deviceInfoDTO.Price,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };

            _context.DeviceInfos.Add(deviceInfo);
            _context.SaveChanges();
        }
    }
}
