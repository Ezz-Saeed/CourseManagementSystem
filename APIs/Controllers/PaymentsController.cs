﻿using APIs.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IPayPalPaymentService payPalPaymentService) : ControllerBase
{

    [HttpPost("createPayment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        var payment = await payPalPaymentService.CreatePaymentIntent(request.TrainerEmail, request.Amount);
        if (payment == null)
            return BadRequest("Trainer not found");

        return Ok(payment);
    }

    [HttpPost("capturePayment/{paymentId}")]
    public async Task<IActionResult> CapturePayment(string paymentId)
    {
        var payment = await payPalPaymentService.CapturePayment(paymentId);
        if (payment == null)
            return BadRequest("Payment not found");

        return Ok(payment);
    }

    [HttpGet("trainerTransactions/{trainerEmail}")]
    public async Task<IActionResult> GetTrainerTransactions(string trainerEmail)
    {
        var transactions = await payPalPaymentService.GetTrainerTransactions(trainerEmail);
        return Ok(transactions);
    }

    //[HttpPost("create")]
    //public IActionResult CreatePayment([FromBody] PaymentRequest request)
    //{
    //    var payment = _payPalService.CreatePayment(request.Amount, request.Currency);

    //    var approvalUrl = payment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;
    //    return Ok(new { PaymentId = payment.id, ApprovalUrl = approvalUrl });
    //}

    //[HttpPost("capture")]
    //public IActionResult CapturePayment([FromBody] CaptureRequest request)
    //{
    //    var payment = _payPalService.CapturePayment(request.PaymentId, request.PayerId);
    //    return Ok(new { Message = "Payment successful!", PaymentId = payment.id });
    //}

    //[HttpGet("transactions")]
    //public IActionResult GetTrainerTransactions()
    //{
    //    var transactions = _payPalService.GetTrainerTransactions();
    //    return Ok(transactions);
    //}
}

public class PaymentRequest
{
    public string TrainerEmail { get; set; }
    public decimal Amount { get; set; }
}

public class CaptureRequest
{
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
}
