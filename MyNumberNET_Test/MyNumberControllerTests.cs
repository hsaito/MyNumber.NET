using Xunit;
using MyNumberNET_ApiServer.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MyNumberNET_Test
{
    public class MyNumberControllerTests
    {
        [Fact]
        public void Verify_ValidNumber_ReturnsTrue()
        {
            var controller = new MyNumberController();
            var validNumber = new int[] { 1,2,3,4,5,6,7,8,9,0,1,2 }; // Replace with a truly valid number if needed
            var result = controller.Verify(validNumber);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public void Verify_InvalidNumber_ReturnsFalse()
        {
            var controller = new MyNumberController();
            var invalidNumber = new int[] { 1,2,3,4,5,6,7,8,9,0,1,3 }; // Last digit is wrong
            var result = controller.Verify(invalidNumber);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public void Verify_MalformedNumber_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var malformedNumber = new int[] { 1,2,3 }; // Too short
            var result = controller.Verify(malformedNumber);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void CheckDigit_ValidInput_ReturnsDigit()
        {
            var controller = new MyNumberController();
            var digits = new int[] { 1,2,3,4,5,6,7,8,9,0,1 }; // Replace with valid digits if needed
            var result = controller.CheckDigit(digits);
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.True(int.TryParse(okResult.Value.ToString(), out _));
        }

        [Fact]
        public void CheckDigit_MalformedInput_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var malformedDigits = new int[] { 1,2,3 }; // Too short
            var result = controller.CheckDigit(malformedDigits);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}

