using System;
using Xunit;
using MyNumberNET_ApiServer.Controllers;
using Microsoft.AspNetCore.Mvc;
using MyNumberNET;

namespace MyNumberNET_Test
{
    public class MyNumberControllerTests
    {
        private readonly int[] _validDigits = { 6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0, 0 };
        private readonly int[] _validFirst11Digits = { 6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0 };

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

        [Fact]
        public void VerifyString_ValidString_ReturnsMyNumberValue()
        {
            var controller = new MyNumberController();
            var result = controller.VerifyString("614106526000");
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var myNumberValue = okResult.Value as MyNumberValue?;
            Assert.True(myNumberValue.HasValue);
            Assert.True(myNumberValue.Value.IsInitialized);
        }

        [Fact]
        public void VerifyString_ValidStringWithSeparators_ReturnsMyNumberValue()
        {
            var controller = new MyNumberController();
            var result = controller.VerifyString("6141-0652-6000");
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var myNumberValue = okResult.Value as MyNumberValue?;
            Assert.True(myNumberValue.HasValue);
            Assert.True(myNumberValue.Value.IsInitialized);
        }

        [Fact]
        public void VerifyString_InvalidString_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var result = controller.VerifyString("invalid");
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public void Create_ValidDigits_ReturnsMyNumberValue()
        {
            var controller = new MyNumberController();
            var result = controller.Create(_validDigits);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var myNumberValue = okResult.Value as MyNumberValue?;
            Assert.True(myNumberValue.HasValue);
            Assert.True(myNumberValue.Value.IsInitialized);
        }

        [Fact]
        public void Create_InvalidDigits_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var invalidDigits = new[] { 1,2,3,4,5,6,7,8,9,0,1,9 }; // Wrong check digit
            var result = controller.Create(invalidDigits);
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public void Complete_ValidFirst11Digits_ReturnsCompleteMyNumberValue()
        {
            var controller = new MyNumberController();
            var result = controller.Complete(_validFirst11Digits);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var myNumberValue = okResult.Value as MyNumberValue?;
            Assert.True(myNumberValue.HasValue);
            Assert.True(myNumberValue.Value.IsInitialized);
            
            // Verify the check digit is correct
            var digits = myNumberValue.Value.Digits;
            Assert.Equal(0, digits[11]); // Expected check digit for our test data
        }

        [Fact]
        public void Complete_InvalidFirst11Digits_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var invalidDigits = new[] { 1,2,3 }; // Too short
            var result = controller.Complete(invalidDigits);
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }

        [Fact]
        public void Generate_Always_ReturnsValidMyNumberValue()
        {
            var controller = new MyNumberController();
            
            // Test multiple generations to ensure consistency
            for (int i = 0; i < 10; i++)
            {
                var result = controller.Generate();
                Assert.NotNull(result.Result);
                var okResult = result.Result as OkObjectResult;
                Assert.NotNull(okResult);
                var myNumberValue = okResult.Value as MyNumberValue?;
                Assert.True(myNumberValue.HasValue);
                Assert.True(myNumberValue.Value.IsInitialized);
                
                // Verify it's actually valid
                Assert.True(MyNumber.VerifyNumber(myNumberValue.Value.Digits));
            }
        }

        [Fact]
        public void Format_ValidNumberWithDefaultFormat_ReturnsFormattedString()
        {
            var controller = new MyNumberController();
            var request = new FormatRequest { Number = "614106526000" };
            var result = controller.Format(request);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("614106526000", okResult.Value);
        }

        [Fact]
        public void Format_ValidNumberWithSpaceFormat_ReturnsFormattedString()
        {
            var controller = new MyNumberController();
            var request = new FormatRequest { Number = "614106526000", Format = "S" };
            var result = controller.Format(request);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("6141 0652 6000", okResult.Value);
        }

        [Fact]
        public void Format_ValidNumberWithHyphenFormat_ReturnsFormattedString()
        {
            var controller = new MyNumberController();
            var request = new FormatRequest { Number = "614106526000", Format = "H" };
            var result = controller.Format(request);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("6141-0652-6000", okResult.Value);
        }

        [Fact]
        public void Format_ValidNumberWithGroupedFormat_ReturnsFormattedString()
        {
            var controller = new MyNumberController();
            var request = new FormatRequest { Number = "614106526000", Format = "G" };
            var result = controller.Format(request);
            Assert.NotNull(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal("6141-0652-600-0", okResult.Value);
        }

        [Fact]
        public void Format_InvalidNumber_ReturnsBadRequest()
        {
            var controller = new MyNumberController();
            var request = new FormatRequest { Number = "invalid" };
            var result = controller.Format(request);
            Assert.NotNull(result.Result);
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
        }
    }
}
