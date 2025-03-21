namespace APIs.Models
{
    public class TrainerPayment
    {
        public int Id { get; set; } 
        public string TrainerId { get; set; } 
        public string TrainerEmail { get; set; } 
        public string PaymentId { get; set; } 
        public decimal Amount { get; set; } 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
