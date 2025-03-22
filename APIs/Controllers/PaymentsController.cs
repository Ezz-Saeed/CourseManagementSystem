using APIs.DTOs.CourseDtos;
using APIs.DTOs.PaymentDtos;
using APIs.Interfaces;
using APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IPayPalPaymentService payPalPaymentService) : ControllerBase
{

    [HttpPost("createPayment")]
    [Authorize(Roles ="Admin")]
    [ProducesResponseType(typeof(TrainerPayment), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
    {
        var payment = await payPalPaymentService.CreatePaymentIntent(request.TrainerEmail, request.Amount);
        if (payment == null)
            return BadRequest("Trainer not found");

        return Ok(payment);
    }

    [HttpPost("capturePayment/{paymentId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GetCourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AddCourseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapturePayment(string paymentId)
    {
        var payment = await payPalPaymentService.CapturePayment(paymentId);
        if (payment == null)
            return BadRequest("Payment not found");

        return Ok(payment);
    }

    [HttpGet("trainerTransactions/{trainerEmail}")]
    [Authorize(Policy = "AdminOrTrainer")]
    [ProducesResponseType(typeof(List<TrainerPayment>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrainerTransactions(string trainerEmail)
    {
        var transactions = await payPalPaymentService.GetTrainerTransactions(trainerEmail);
        return Ok(transactions);
    }
}
