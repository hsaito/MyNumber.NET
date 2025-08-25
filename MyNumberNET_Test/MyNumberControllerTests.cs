using System;
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
            var first11 = new[] { 1,2,3,4,5,6,7,8,9,0,1 };
            var checkDigit = MyNumberNET.MyNumber.CalculateCheckDigits((int[])first11.Clone());
            var validNumber = new int[12];
            Array.Copy(first11, validNumber, 11);
            validNumber[11] = checkDigit;
            var result = controller.Verify(validNumber);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public void Verify_InvalidNumber_ReturnsFalse()
        {
            var controller = new MyNumberController();
            var invalidNumber = new[] { 1,2,3,4,5,6,7,8,9,0,1,3 }; // Last digit is wrong
            var result = controller.Verify(invalidNumber);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public void Verify_MalformedNumber_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var malformedNumber = new[] { 1,2,3 }; // Too short
            var result = controller.Verify(malformedNumber);
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public void CheckDigit_ValidInput_ReturnsDigit()
        {
            var controller = new MyNumberController();
            var digits = new[] { 1,2,3,4,5,6,7,8,9,0,1 };
            var result = controller.CheckDigit(digits);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.True(int.TryParse(okResult.Value.ToString(), out _));
        }

        [Fact]
        public void CheckDigit_MalformedInput_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var malformedDigits = new[] { 1,2,3 }; // Too short
            var result = controller.CheckDigit(malformedDigits);
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }
    }
}
