using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IStripeService
    {
        Task<string> CreateCharge();
    }
    public class StripeService : IStripeService
    {
        private readonly TokenService _tokenService;
        private readonly CustomerService _customerService;
        private readonly ChargeService _chargeService;

        public StripeService(
            TokenService tokenService,
            CustomerService customerService,
            ChargeService chargeService)
        {
            _tokenService = tokenService;
            _customerService = customerService;
            _chargeService = chargeService;
        }

        public async Task<string> CreateCharge()
        {
            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = "Test",
                    Number = "4242424242424242",
                    ExpYear = "2028",
                    ExpMonth = "03",
                    Cvc = "123"
                }
            };

            // Create new Stripe Token
            Token stripeToken = await _tokenService.CreateAsync(tokenOptions);

            // Set Customer options using
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = "Test",
                Email = "test@mailinator.com",
                Source = stripeToken.Id
            };

            // Create customer at Stripe
            Customer createdCustomer = await _customerService.CreateAsync(customerOptions);

            ChargeCreateOptions paymentOptions = new ChargeCreateOptions
            {
                Customer = createdCustomer.Id,
                ReceiptEmail = createdCustomer.Email,
                Description = "",
                Currency = "usd",
                Amount = 20 * 100
            };

            var createdPayment = await _chargeService.CreateAsync(paymentOptions);

            return createdPayment.Status;
        }
    }
}
