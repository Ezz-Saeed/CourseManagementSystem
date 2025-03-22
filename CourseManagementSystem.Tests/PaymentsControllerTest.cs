using APIs.DTOs.PaymentDtos;
using APIs.Interfaces;
using APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagementSystem.Tests
{
    public class PaymentsControllerTest
    {
        private readonly Mock<IPayPalPaymentService> _payPalPaymentServiceMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTest()
        {
            _payPalPaymentServiceMock = new Mock<IPayPalPaymentService>();
            _controller = new PaymentsController(_payPalPaymentServiceMock.Object);
        }

        [Fact]
        public async Task CreatePayment_ReturnsOk_WhenPaymentCreated()
        {
            // Arrange
            var request = new PaymentRequestDto { TrainerEmail = "trainer@example.com", Amount = 100m };
            var expectedPayment = new TrainerPayment { TrainerEmail = request.TrainerEmail, Amount = request.Amount, Status = "Pending" };

            _payPalPaymentServiceMock
                .Setup(s => s.CreatePaymentIntent(request.TrainerEmail, request.Amount))
                .ReturnsAsync(expectedPayment);

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(expectedPayment, okResult.Value);
        }

        [Fact]
        public async Task CreatePayment_ReturnsBadRequest_WhenTrainerNotFound()
        {
            // Arrange
            var request = new PaymentRequestDto { TrainerEmail = "nonexistent@example.com", Amount = 100m };

            _payPalPaymentServiceMock
                .Setup(s => s.CreatePaymentIntent(request.TrainerEmail, request.Amount))
                .ReturnsAsync((TrainerPayment)null);

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Trainer not found", badRequestResult.Value);
        }


        [Fact]
        public async Task CapturePayment_ReturnsOk_WhenPaymentCaptured()
        {
            // Arrange
            var paymentId = "PAY-123";
            var expectedPayment = new TrainerPayment { PaymentId = paymentId, Status = "Completed" };

            _payPalPaymentServiceMock
                .Setup(s => s.CapturePayment(paymentId))
                .ReturnsAsync(expectedPayment);

            // Act
            var result = await _controller.CapturePayment(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(expectedPayment, okResult.Value);
        }

        [Fact]
        public async Task CapturePayment_ReturnsBadRequest_WhenPaymentNotFound()
        {
            // Arrange
            var paymentId = "INVALID-PAY-ID";

            _payPalPaymentServiceMock
                .Setup(s => s.CapturePayment(paymentId))
                .ReturnsAsync((TrainerPayment)null);

            // Act
            var result = await _controller.CapturePayment(paymentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Payment not found", badRequestResult.Value);
        }


        [Fact]
        public async Task GetTrainerTransactions_ReturnsOk_WithTransactions()
        {
            // Arrange
            var trainerEmail = "trainer@example.com";
            var expectedTransactions = new List<TrainerPayment>
        {
            new TrainerPayment { TrainerEmail = trainerEmail, Amount = 50m, Status = "Completed" },
            new TrainerPayment { TrainerEmail = trainerEmail, Amount = 100m, Status = "Pending" }
        };

            _payPalPaymentServiceMock
                .Setup(s => s.GetTrainerTransactions(trainerEmail))
                .ReturnsAsync(expectedTransactions);

            // Act
            var result = await _controller.GetTrainerTransactions(trainerEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(expectedTransactions, okResult.Value);
        }

        [Fact]
        public async Task GetTrainerTransactions_ReturnsOk_WithEmptyList_WhenNoTransactionsFound()
        {
            // Arrange
            var trainerEmail = "trainer@example.com";

            _payPalPaymentServiceMock
                .Setup(s => s.GetTrainerTransactions(trainerEmail))
                .ReturnsAsync(new List<TrainerPayment>());

            // Act
            var result = await _controller.GetTrainerTransactions(trainerEmail);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var transactions = Assert.IsType<List<TrainerPayment>>(okResult.Value);
            Assert.Empty(transactions);
        }
    }
}   
