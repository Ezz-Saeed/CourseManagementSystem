using APIs.Data;
using APIs.Helpers;
using APIs.Interfaces;
using APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace APIs.Services
{
    public class PayPalPaymentService(UserManager<Appuser> userManager, 
        AppDbContext context, IOptions<PaypalCredentials> options) : IPayPalPaymentService
    {
        private readonly PaypalCredentials paypalCredentials = options.Value;
        //private readonly string _clientId = configuration["PayPal:ClientId"];
        //private readonly string _clientSecret = configuration["PayPal:ClientSecret"];
        //private readonly string _mode = configuration["PayPal:Mode"];

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", paypalCredentials.Mode }
            };
            var accessToken = new OAuthTokenCredential(paypalCredentials.ClientId, paypalCredentials.ClientSecret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public async Task<TrainerPayment> CreatePaymentIntent(string trainerEmail, decimal amount)
        {
            var apiContext = GetAPIContext();
            var trainer = await userManager.FindByEmailAsync(trainerEmail);

            if (trainer is null) return null;

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount
                        {
                            total = amount.ToString("F2"),
                            currency = "USD"
                        },
                        description = $"Payment for Trainer: {trainerEmail}"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = "https://yourapp.com/success",
                    cancel_url = "https://yourapp.com/cancel"
                }
            };

            var createdPayment = payment.Create(apiContext);

            var trainerPayment = new TrainerPayment
            {
                TrainerId = trainer.Id,
                TrainerEmail = trainer.Email,
                PaymentId = createdPayment.id,
                Amount = amount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await context.TrainerPayments.AddAsync(trainerPayment);
            await context.SaveChangesAsync();

            return trainerPayment;
        }

    }
}
