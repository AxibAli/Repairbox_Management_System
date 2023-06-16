using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.CustomerProductPurchase;
using RepairBox.BL.DTOs.DeviceInfo;
using RepairBox.BL.DTOs.Model;
using RepairBox.BL.DTOs.PurchaseFromCustomerInvoiceDTOs;
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
        public GetInvoiceDataDTO? GetPurchaseFromCustomerInvoice(long id);
        public int AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO);
        public void AddPurchaseFromCustomerInvoice(AddPurchaseFromCustomerInvoiceDTO purchaseFromCustomerInvoiceDTO);
        public bool DeletePurchaseFromCustomer(long id);
        public GetPurchaseFromCustomerInvoiceDTO? UpdatePurchaseFromCustomer(UpdateCustomerInfoDTO customerInfoDTO, UpdateCustomerIdentitiesDTO customerIdentitiesDTO, UpdateDeviceInfoDTO deviceInfoDTO);
    }
    public class CustomerProductPurchaseServiceRepo : ICustomerProductPurchaseServiceRepo
    {
        public ApplicationDBContext _context;
        public CustomerProductPurchaseServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public GetInvoiceDataDTO? GetPurchaseFromCustomerInvoice(long id)
        {
            var invoice = _context.PurchaseFromCustomerInvoices.FirstOrDefault(i => i.invoiceId == id);
            if (invoice != null)
            {
                var customerInfo = _context.CustomerInfos.FirstOrDefault(c => c.Id == invoice.CustomerInfoId);
                var deviceInfo = _context.DeviceInfos.FirstOrDefault(d => d.CustomerInfoId == invoice.CustomerInfoId);

                var dto = new GetInvoiceDataDTO();
                dto.CustomerInfo = Omu.ValueInjecter.Mapper.Map<GetCustomerInfoDTO>(customerInfo);
                dto.DeviceInfo = Omu.ValueInjecter.Mapper.Map<GetDeviceInfoDTO>(deviceInfo);
                dto.PurchaseFromCustomerInvoice = Omu.ValueInjecter.Mapper.Map<GetPurchaseFromCustomerInvoiceDTO>(invoice);

                return dto;
            }

            return null;
        }
        public int AddCustomerProductPurchase(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO)
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

            return customerInfo.Id;
        }
        public void AddPurchaseFromCustomerInvoice(AddPurchaseFromCustomerInvoiceDTO purchaseFromCustomerInvoiceDTO)
        {
            var purchaseFromCustomerInvoice = new PurchaseFromCustomerInvoice
            {
                invoiceId = purchaseFromCustomerInvoiceDTO.invoiceId,
                CustomerInfoId = purchaseFromCustomerInvoiceDTO.CustomerInfoId,
                Date = purchaseFromCustomerInvoiceDTO.Date,
                QRCodePath = purchaseFromCustomerInvoiceDTO.QRCodePath,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };

            _context.PurchaseFromCustomerInvoices.Add(purchaseFromCustomerInvoice);
            _context.SaveChanges();
        }
        public bool DeletePurchaseFromCustomer(long id)
        {
            var invoice = _context.PurchaseFromCustomerInvoices.FirstOrDefault(i => i.invoiceId == id);
            
            if(invoice == null) { return false; }

            invoice.IsDeleted = true;
            invoice.IsActive = false;

            var deviceInfo = _context.DeviceInfos.FirstOrDefault(d => d.CustomerInfoId == invoice.CustomerInfoId);
            deviceInfo.IsDeleted = true;
            deviceInfo.IsActive = false;

            var customerIdentities = _context.CustomerIdentities.FirstOrDefault(ci => ci.CustomerInfoId == invoice.CustomerInfoId);
            customerIdentities.IsDeleted = true;
            customerIdentities.IsActive = false;

            var customerInfo = _context.CustomerInfos.FirstOrDefault(c => c.Id == invoice.CustomerInfoId);
            customerInfo.IsDeleted = true;
            customerInfo.IsActive = false;

            _context.SaveChanges();
            return true;
        }
        public GetPurchaseFromCustomerInvoiceDTO? UpdatePurchaseFromCustomer(UpdateCustomerInfoDTO customerInfoDTO, UpdateCustomerIdentitiesDTO customerIdentitiesDTO, UpdateDeviceInfoDTO deviceInfoDTO)
        {
            var invoice = _context.PurchaseFromCustomerInvoices.FirstOrDefault(i => i.CustomerInfoId == customerInfoDTO.Id);

            if (invoice == null) { return null; }

            var customerInfo = _context.CustomerInfos.FirstOrDefault(c => c.Id == customerInfoDTO.Id);

            customerInfo.Name = customerInfoDTO.Name;
            customerInfo.Email = customerInfoDTO.Email;
            customerInfo.PhoneNumber = customerInfoDTO.PhoneNumber;
            customerInfo.Address = customerInfoDTO.Address;

            var customerIdentities = _context.CustomerIdentities.FirstOrDefault(ci => ci.CustomerInfoId == customerInfoDTO.Id);

            customerIdentities.Image = customerIdentitiesDTO.Image;

            var deviceInfo = _context.DeviceInfos.FirstOrDefault(d => d.CustomerInfoId == customerInfoDTO.Id);

            deviceInfo.DeviceNameModel = deviceInfoDTO.DeviceNameModel;
            deviceInfo.IMEI = deviceInfoDTO.IMEI;
            deviceInfo.SerialNumber = deviceInfoDTO.SerialNumber;
            deviceInfo.Cost = deviceInfoDTO.Cost;
            deviceInfo.Price = deviceInfoDTO.Price;

            _context.SaveChanges();

            return Omu.ValueInjecter.Mapper.Map<GetPurchaseFromCustomerInvoiceDTO>(invoice);
        }
    }
}
