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
        Task AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO);
    }
    public class CustomerProductPurchaseServiceRepo : ICustomerProductPurchaseServiceRepo
    {
        public ApplicationDBContext _context;
        public CustomerProductPurchaseServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO)
        {
            var customerInfo = new CustomerInfo
            {
                Name = customerInfoDTO.Name,
                Email = customerInfoDTO.Email,
                PhoneNumber = customerInfoDTO.PhoneNumber,
                Address = customerInfoDTO.Address
            };

            await _context.CustomerInfos.AddAsync(customerInfo);
            await _context.SaveChangesAsync();

            var customerIdentities = new CustomerIdentities
            {
                CustomerInfoId = customerInfo.Id,
                Image1 = customerIdentitiesDTO.Image1,
                Image2 = customerIdentitiesDTO.Image2
            };

            await _context.CustomerIdentities.AddAsync(customerIdentities);
            await _context.SaveChangesAsync();

            var deviceInfo = new DeviceInfo
            {
                CustomerInfoId = customerInfo.Id,
                DeviceNameModel = deviceInfoDTO.DeviceNameModel,
                IMEI = deviceInfoDTO.IMEI,
                SerialNumber = deviceInfoDTO.SerialNumber,
                Cost = deviceInfoDTO.Cost,
                Price = deviceInfoDTO.Price
            };

            await _context.DeviceInfos.AddAsync(deviceInfo);
            await _context.SaveChangesAsync();
        }
    }
}
