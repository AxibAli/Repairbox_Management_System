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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IOrderServiceRepo
    {
        CalculateOrderAmountDTO CalculateOrder(GetOrderChargesDTO model);
        Task CreateOrder(AddOrderDTO model);
        PaginationModel GetOrderList(int pageNo, string? query);
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
        public PaginationModel GetOrderList(int pageNo, string? query)
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
                    StatusName = result.Status.Name
                })
                .ToList();

            var TotalPages = orderList.Count;

            orderList = orderList.Skip(pageNo * 10).Take(10).ToList();

            return new PaginationModel
            {
                TotalPages = CommonHelper.TotalPagesforPagination(TotalPages, 10),
                CurrentPage = pageNo,
                Data = orderList
            };
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
