using Microsoft.AspNetCore.Mvc;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.CustomerIdentities;
using RepairBox.BL.DTOs.CustomerInfo;
using RepairBox.BL.DTOs.DeviceInfo;
using RepairBox.BL.DTOs.CustomerProductPurchase;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;
using System.Web;
using IronBarCode;
using System.Text;
using RepairBox.BL.DTOs.PurchaseFromCustomerInvoiceDTOs;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class CustomerProductPurchaseController : ControllerBase
    {
        private ICustomerProductPurchaseServiceRepo _customerProductPurchaseRepo;
        public CustomerProductPurchaseController(ICustomerProductPurchaseServiceRepo customerProductPurchaseRepo)
        {
            _customerProductPurchaseRepo = customerProductPurchaseRepo;
        }

        [HttpGet("GetPurchaseFromCustomerInvoice")]
        public IActionResult GetPurchaseFromCustomerInvoice(long id)
        {
            try
            {
                var data = _customerProductPurchaseRepo.GetPurchaseFromCustomerInvoice(id);
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("AddCustomerProductPurchase")]
        public IActionResult AddCustomerProductPurchase(AddCustomerProductPurchaseDTO addCustomerProductPurchase)
        {
            AddCustomerInfoDTO customerInfo = addCustomerProductPurchase.CustomerInfo;
            AddCustomerIdentitiesDTO customerIdentities = addCustomerProductPurchase.CustomerIdentities;
            AddDeviceInfoDTO deviceInfo = addCustomerProductPurchase.DeviceInfo;

            try
            {
                int customerInfoId = _customerProductPurchaseRepo.AddCustomerProductPurchase(customerInfo, customerIdentities, deviceInfo);

                DateTime currentDate = DateTime.Now;
                string formattedDate = currentDate.ToString("dd-MM-yyyy HH:mm:ss");

                var random = new Random();
                long invoiceId;
                long minValue = 1000000000;
                long maxValue = 9999999999;

                do
                {
                    byte[] buffer = new byte[8];
                    random.NextBytes(buffer);

                    long longRand = BitConverter.ToInt64(buffer, 0);
                    invoiceId = Math.Abs(longRand % (maxValue - minValue)) + minValue;
                } while (_customerProductPurchaseRepo.GetPurchaseFromCustomerInvoice(invoiceId) != null);

                var data = String.Format("Company: RepairBox\nCompanyPhone: 0123456789\nCompanyAddress: Pakistan\nInvoice Number: #INV-{0}\nInvoice Date: {1}\nClient Name: {2}\nClient Address: {3}\nDevice Model: {4}\nDevice IMEI: {5}\nDevice Serial Number: {6}\nDevice Quantity: {7}\nTotal Amount: {8}",invoiceId,formattedDate,customerInfo.Name,customerInfo.Address,deviceInfo.DeviceNameModel,deviceInfo.IMEI,deviceInfo.SerialNumber,1, deviceInfo.Price);

                var QRCodePath = String.Format("wwwroot\\QRCode\\{0}-QR.png", invoiceId);

                QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng(QRCodePath);

                AddPurchaseFromCustomerInvoiceDTO purchaseFromCustomerInvoiceDTO = new AddPurchaseFromCustomerInvoiceDTO();
                purchaseFromCustomerInvoiceDTO.invoiceId = invoiceId;
                purchaseFromCustomerInvoiceDTO.CustomerInfoId = customerInfoId;
                purchaseFromCustomerInvoiceDTO.Date = currentDate;
                purchaseFromCustomerInvoiceDTO.QRCodePath = QRCodePath;

                _customerProductPurchaseRepo.AddPurchaseFromCustomerInvoice(purchaseFromCustomerInvoiceDTO);

                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Data = invoiceId.ToString(), Message = String.Format(CustomMessage.ADDED_SUCCESSFULLY, "Customer Product Purchase") });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }
    }
}
