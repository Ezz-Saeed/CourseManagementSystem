using APIs.Models;
using PayPal.Api;

namespace APIs.Interfaces
{
    public interface IPayPalPaymentService
    {
        Task<TrainerPayment> CreatePaymentIntent(string trainerEmail, decimal amount);
        Task<TrainerPayment> CapturePayment(string paymentId);
        Task<List<TrainerPayment>> GetTrainerTransactions(string trainerEmail);
    }
}
