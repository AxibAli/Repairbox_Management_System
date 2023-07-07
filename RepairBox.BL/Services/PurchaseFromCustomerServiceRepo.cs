using Microsoft.EntityFrameworkCore;
using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.CustomerProductPurchase;
using RepairBox.BL.DTOs.DeviceInfo;
using RepairBox.BL.DTOs.Model;
using RepairBox.BL.DTOs.PurchaseFromCustomer;
using RepairBox.BL.DTOs.PurchaseFromCustomerInvoiceDTOs;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RepairBox.BL.Services
{
    public interface IPurchaseFromCustomerServiceRepo
    {
        public GetInvoiceDataDTO? GetPurchaseFromCustomerInvoice(long id);
        public PaginationModel GetPurchaseFromCustomerListing(int pageNo, string? query);
        public int AddPurchaseFromCustomer(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO);
        public void AddPurchaseFromCustomerInvoice(AddPurchaseFromCustomerInvoiceDTO purchaseFromCustomerInvoiceDTO);
        public bool DeletePurchaseFromCustomer(long id);
        public GetPurchaseFromCustomerInvoiceDTO? UpdatePurchaseFromCustomer(UpdateCustomerInfoDTO customerInfoDTO, UpdateCustomerIdentitiesDTO customerIdentitiesDTO, UpdateDeviceInfoDTO deviceInfoDTO, long invoiceId);
    }
    public class PurchaseFromCustomerServiceRepo : IPurchaseFromCustomerServiceRepo
    {
        public ApplicationDBContext _context;
        public PurchaseFromCustomerServiceRepo(ApplicationDBContext context)
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
        public PaginationModel GetPurchaseFromCustomerListing(int pageNo, string? query)
        {
            var purchaseFromCustomerList = _context.PurchaseFromCustomerInvoices
                .Join(
                    _context.CustomerInfos,
                    invoice => invoice.CustomerInfoId,
                    customer => customer.Id,
                    (invoice, customer) => new { Invoice = invoice, Customer = customer }
                )
                .Join(
                    _context.DeviceInfos,
                    joinResult => joinResult.Customer.Id,
                    deviceInfo => deviceInfo.CustomerInfoId,
                    (joinResult, deviceInfo) => new { joinResult.Invoice, joinResult.Customer, DeviceInfo = deviceInfo }
                )
                .Select(result => new GetPurchaseFromCustomerDTO
                {
                    Id = result.Invoice.Id,
                    InvoiceId = result.Invoice.invoiceId,
                    CustomerName = result.Customer.Name,
                    CustomerEmail = result.Customer.Email,
                    DeviceNameModel = result.DeviceInfo.DeviceNameModel,
                    DeviceIMEI = result.DeviceInfo.IMEI,
                    DeviceSerialNumber = result.DeviceInfo.SerialNumber,
                    DeviceCost = result.DeviceInfo.Cost,
                    DevicePrice = result.DeviceInfo.Price,
                    Date = result.Invoice.Date,
                    QRCodePath = result.Invoice.QRCodePath
                })
                .ToList();

            var TotalPages = purchaseFromCustomerList.Count;

            purchaseFromCustomerList = purchaseFromCustomerList.Skip(pageNo * 10).Take(10).ToList();

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(TotalPages, 10),
                CurrentPage = pageNo,
                Data = purchaseFromCustomerList
            };
        }
        public int AddPurchaseFromCustomer(AddCustomerInfoDTO customerInfoDTO, AddCustomerIdentitiesDTO customerIdentitiesDTO, AddDeviceInfoDTO deviceInfoDTO)
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
        public GetPurchaseFromCustomerInvoiceDTO? UpdatePurchaseFromCustomer(UpdateCustomerInfoDTO customerInfoDTO, UpdateCustomerIdentitiesDTO customerIdentitiesDTO, UpdateDeviceInfoDTO deviceInfoDTO, long invoiceId)
        {
            var invoice = _context.PurchaseFromCustomerInvoices.FirstOrDefault(i => i.invoiceId == invoiceId);

            if (invoice == null) { return null; }

            var customerInfo = _context.CustomerInfos.FirstOrDefault(c => c.Id == invoice.CustomerInfoId);

            customerInfo.Name = customerInfoDTO.Name;
            customerInfo.Email = customerInfoDTO.Email;
            customerInfo.PhoneNumber = customerInfoDTO.PhoneNumber;
            customerInfo.Address = customerInfoDTO.Address;

            var customerIdentities = _context.CustomerIdentities.FirstOrDefault(ci => ci.CustomerInfoId == invoice.CustomerInfoId);

            customerIdentities.Image = customerIdentitiesDTO.Image;

            var deviceInfo = _context.DeviceInfos.FirstOrDefault(d => d.CustomerInfoId == invoice.CustomerInfoId);

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
