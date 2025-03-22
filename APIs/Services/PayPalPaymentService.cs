using APIs.Data;
using APIs.Helpers;
using APIs.Interfaces;
using APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace APIs.Services
{
    public class PayPalPaymentService(UserManager<Appuser> userManager, 
        AppDbContext context, IOptions<PaypalCredentials> options) : IPayPalPaymentService
    {
        // Configuring object to get Papal data
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

        // Method to Create payment for a trainer
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
            // Inserting trainer payment after payment succeeded
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

        // Capture payment with id to update TrainerPayment status
        public async Task<TrainerPayment> CapturePayment(string paymentId)
        {
            var apiContext = GetAPIContext();

                var payment = Payment.Get(apiContext, paymentId);
                var trainerPayment = await context.TrainerPayments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
                if (trainerPayment is null) return null;

                // Update payment status in the database
                trainerPayment.Status = payment.state == "created" ? "Completed" : "Failed";
                trainerPayment.CompletedAt = DateTime.UtcNow;

                context.TrainerPayments.Update(trainerPayment);
                await context.SaveChangesAsync();

                return trainerPayment;           
        }

        public async Task<List<TrainerPayment>> GetTrainerTransactions(string trainerEmail)
        {
            return await context.TrainerPayments.Where(p => p.TrainerEmail == trainerEmail).ToListAsync();
        }

    }
}
