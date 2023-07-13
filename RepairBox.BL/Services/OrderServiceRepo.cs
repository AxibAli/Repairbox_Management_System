using BitMiracle.LibTiff.Classic;
using Microsoft.EntityFrameworkCore;
using RepairBox.BL.DTOs.Order;
using RepairBox.BL.DTOs.Stripe;
using RepairBox.BL.DTOs.User;
using RepairBox.BL.ServiceModels.Order;
using RepairBox.Common.Commons;
using RepairBox.Common.Helpers;
using RepairBox.DAL;
using RepairBox.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IOrderServiceRepo
    {
        CalculateOrderAmountDTO CalculateOrder(GetOrderChargesDTO model);
        Task CreateOrder(AddOrderDTO model);
        PaginationModel GetOrderList(int pageNo, DisplayOrderFiltersDTO? orderFilters);
    }

    public class OrderServiceRepo : IOrderServiceRepo
    {
        public ApplicationDBContext _context;
        public IStripeService _stripeRepo;

        private int pageSize = DeveloperConstants.PAGE_SIZE;
        public OrderServiceRepo(ApplicationDBContext context, IStripeService stripeRepo)
        {
            _context = context;
            _stripeRepo = stripeRepo;

        }
        public CalculateOrderAmountDTO CalculateOrder(GetOrderChargesDTO model)
        {
            decimal subTotal = 0;
            decimal tax = 0;
            var defects = _context.RepairableDefects.Where(d => model.Defects.Contains(d.Id)).ToList();
            defects.ForEach(d => subTotal += d.Price);

            var setting = _context.Settings.FirstOrDefault();
            if (setting != null)
                tax = setting.Tax;

            var priorityProcessCharges = _context.RepairPriorities.FirstOrDefault(p => p.Id == model.PriorityId)?.ProcessCharges;

            decimal totalAmount = subTotal + tax + priorityProcessCharges.Value;

            return new CalculateOrderAmountDTO
            {
                SubTotal = subTotal,
                Tax = tax,
                PriorityProcessCharges = priorityProcessCharges.Value,
                TotalAmount = totalAmount
            };
        }
        public async Task CreateOrder(AddOrderDTO model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (model.PaymentMethod == enPaymentMethod.Card)
                {
                    var stripeResponse = await _stripeRepo.CreateCharge(new StripeRequestDTO
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        CardNumber = model.CardNumber,
                        ExpiryMonth = model.ExpiryMonth,
                        ExpiryYear = model.ExpiryYear,
                        CVV = model.CVV,
                        CardType = model.CardType,
                        Amount = model.Amount
                    });
                    if (stripeResponse.Status == enStripeChargeStatus.succeeded)
                    {
                        AddTransaction(new Transaction
                        {
                            Amount = model.Amount,
                            CardMask = model.CardNumber.Substring(model.CardNumber.Length - 4),
                            CardType = model.CardType,
                            Email = model.Email,
                            CreatedAt = DateTime.UtcNow,
                            StripeCustomerId = stripeResponse.StripeCustomerId,
                            StripeTransactionId = stripeResponse.StripeTransactionId,
                            IsActive = true,
                            IsDeleted = false
                        });

                        int orderId = AddOrder(model);
                        AddOrderDefects(model.RepairableDefects, orderId);
                    }
                }
                else
                {
                    int orderId = AddOrder(model);
                    AddOrderDefects(model.RepairableDefects, orderId);
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
        public PaginationModel GetOrderList(int pageNo, DisplayOrderFiltersDTO? orderFilters)
        {
            var orderList = _context.Orders
                .Join(_context.Models, order => order.ModelId, model => model.Id, (order, model) => new { Order = order, Model = model })
                .Join(_context.Brands, joinResult => joinResult.Model.BrandId, brand => brand.Id, (joinResult, brand) => new { joinResult.Order, joinResult.Model, Brand = brand })
                .Join(_context.Users, joinResult => joinResult.Order.TechnicianId, technician => technician.Id, (joinResult, technician) => new { joinResult.Order, joinResult.Model, joinResult.Brand, Technician = technician })
                .Join(_context.RepairPriorities, joinResult => joinResult.Order.PriorityId, priority => priority.Id, (joinResult, priority) => new { joinResult.Order, joinResult.Model, joinResult.Brand, joinResult.Technician, Priority = priority })
                .Join(_context.RepairStatuses, joinResult => joinResult.Order.StatusId, status => status.Id, (joinResult, status) => new { joinResult.Order, joinResult.Model, joinResult.Brand, joinResult.Technician, joinResult.Priority, Status = status })
                .Select(result => new GetOrderDTO
                {
                    Id = result.Order.Id,
                    Name = result.Order.Name,
                    Email = result.Order.Email,
                    Phone = result.Order.Phone,
                    SerialNumber = result.Order.SerialNumber,
                    Address = result.Order.Address,
                    Diagnostics = result.Order.Diagnostics,
                    BrandName = result.Brand.Name,
                    ModelId = result.Model.Id,
                    ModelName = result.Model.Name,
                    TechnicianId = result.Technician.Id,
                    TechnicianName = result.Technician.Username,
                    PriorityId = result.Priority.Id,
                    PriorityName = result.Priority.Name,
                    StatusId = result.Status.Id,
                    StatusName = result.Status.Name,
                    WarrantyStatus = result.Order.WarrantyStatus,
                    CreatedAt = result.Order.CreatedAt,
                    AmountPaid = 0.0M
                })
                .ToList();

            for (int i = 0; i < orderList.Count; i++)
            {
                orderList[i].OrderAmount = CalculateOrder(
                    new GetOrderChargesDTO
                    {
                        Defects = _context.OrderDefects.Where(od => od.OrderId == orderList[i].Id).Select(od => od.RepairableDefectId).ToList(),
                        PriorityId = orderList[i].PriorityId
                    });
            }

            var TotalPages = 0;

            if(orderFilters == null)
            {
                TotalPages = orderList.Count;
                orderList = orderList.Skip((pageNo - 1) * 10).Take(10).ToList();

                return new PaginationModel
                {
                    TotalPages = CommonHelper.TotalPagesforPagination(TotalPages, 10),
                    CurrentPage = pageNo,
                    Data = orderList
                };
            }
            else
            {
                // Apply search filter
                if (!string.IsNullOrEmpty(orderFilters.searchKeyword))
                {
                    orderList = orderList.Where(dto =>
                        dto.Name.Contains(orderFilters.searchKeyword) ||
                        dto.Email.Contains(orderFilters.searchKeyword) ||
                        dto.Phone.Contains(orderFilters.searchKeyword) ||
                        dto.SerialNumber.Contains(orderFilters.searchKeyword) ||
                        dto.Address.Contains(orderFilters.searchKeyword) ||
                        dto.BrandName.Contains(orderFilters.searchKeyword) ||
                        dto.ModelName.Contains(orderFilters.searchKeyword) ||
                        dto.TechnicianName.Contains(orderFilters.searchKeyword) ||
                        dto.Id.ToString() == orderFilters.searchKeyword
                    ).ToList();
                }

                // Apply Technician Filter
                if(orderFilters.technicianId != null)
                {
                    orderList = orderList.Where(dto => dto.TechnicianId == orderFilters.technicianId).ToList();
                }

                // Apply Payment Filter
                if (orderFilters.isPaid != null)
                {
                    orderList = orderList.Where(dto => (dto.AmountPaid >= dto.OrderAmount.TotalAmount) == orderFilters.isPaid).ToList();
                }

                //// Apply Lock state Filter
                //if (orderFilters.isLocked != null)
                //{
                //    orderList = orderList.Where(dto => dto.).ToList();
                //}

                //// Apply Lock state Filter
                //if (orderFilters.isLocked != null)
                //{
                //    orderList = orderList.Where(dto => dto.).ToList();
                //}

                // Apply Warranty Filter
                if (orderFilters.hasWarranty != null)
                {
                    orderList = orderList.Where(dto => dto.WarrantyStatus == orderFilters.hasWarranty).ToList();
                }

                // Apply Status Filter
                if (orderFilters.statusId != null)
                {
                    orderList = orderList.Where(dto => dto.StatusId == orderFilters.statusId).ToList();
                }

                // Apply Priority Filter
                if (orderFilters.priorityId != null)
                {
                    orderList = orderList.Where(dto => dto.PriorityId == orderFilters.priorityId).ToList();
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(orderFilters.sortBy))
                {
                    switch (orderFilters.sortBy)
                    {
                        case "Status":
                            orderList = orderFilters.isSortAscending
                                ? orderList.OrderBy(dto => dto.StatusId).ToList()
                                : orderList.OrderByDescending(dto => dto.StatusId).ToList();
                            break;
                        case "Priority":
                            orderList = orderFilters.isSortAscending
                                ? orderList.OrderBy(dto => dto.PriorityId).ToList()
                                : orderList.OrderByDescending(dto => dto.PriorityId).ToList();
                            break;
                        case "Technician":
                            orderList = orderFilters.isSortAscending
                                ? orderList.OrderBy(dto => dto.TechnicianId).ToList()
                                : orderList.OrderByDescending(dto => dto.TechnicianId).ToList();
                            break;
                        case "Created at":
                            orderList = orderFilters.isSortAscending
                                ? orderList.OrderBy(dto => dto.CreatedAt).ToList()
                                : orderList.OrderByDescending(dto => dto.CreatedAt).ToList();
                            break;
                        case "Order Number":
                            orderList = orderFilters.isSortAscending
                                ? orderList.OrderBy(dto => dto.Id).ToList()
                                : orderList.OrderByDescending(dto => dto.Id).ToList();
                            break;
                        //case "Updated At":
                        //    orderList = orderFilters.isSortAscending
                        //        ? orderList.OrderBy(dto => dto.UpdatedAt).ToList()
                        //        : orderList.OrderByDescending(dto => dto.UpdatedAt).ToList();
                        //    break;
                        //case "Closed At":
                        //    orderList = orderFilters.isSortAscending
                        //        ? orderList.OrderBy(dto => dto.ClosedAt).ToList()
                        //        : orderList.OrderByDescending(dto => dto.ClosedAt).ToList();
                        //    break;

                        default:
                            throw new Exception("Invalid Sort By Value");
                    }
                }

                TotalPages = orderList.Count;
                orderList = orderList.Skip((pageNo - 1) * 10).Take(orderFilters.pageSize).ToList();

                return new PaginationModel
                {
                    TotalPages = CommonHelper.TotalPagesforPagination(TotalPages, orderFilters.pageSize),
                    CurrentPage = pageNo,
                    Data = orderList
                };
            }
        }
        // Private Method
        private void AddTransaction(Transaction transaction)
        {
            try
            {
                _context.Transactions.Add(new Transaction
                {
                    Amount = transaction.Amount,
                    CardMask = transaction.CardMask,
                    CardType = transaction.CardType,
                    Email = transaction.Email,
                    CreatedAt = transaction.CreatedAt,
                    StripeCustomerId = transaction.StripeCustomerId,
                    StripeTransactionId = transaction.StripeTransactionId,
                    IsActive = true,
                    IsDeleted = false
                });
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int AddOrder(AddOrderDTO model)
        {
            try
            {
                var order = new Order
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    CreatedAt = DateTime.Now,
                    Diagnostics = model.Diagnostics,
                    PaymentMethod = model.PaymentMethod,
                    ModelId = model.ModelId,
                    PriorityId = model.PriorityId,
                    StatusId = 0,
                    WarrantyStatus = model.WarrantyStatus,
                    SerialNumber = model.SerialNumber,
                    IsActive = true,
                    IsDeleted = false,
                    TechnicianId = 0,
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                return order.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void AddOrderDefects(List<int> Defects, int OrderId)
        {
            foreach (var defect in Defects)
            {
                _context.OrderDefects.Add(new OrderDefect
                {
                    OrderId = OrderId,
                    RepairableDefectId = defect,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false,
                });

                _context.SaveChanges();
            }
        }
    }
}
