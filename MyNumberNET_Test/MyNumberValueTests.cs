using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNumberNET;

namespace MyNumberNET_Test
{
    [TestClass]
    public class MyNumberValueTests
    {
        private readonly int[] _validMyNumber = { 6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0, 0 };
        private readonly int[] _invalidMyNumber = { 6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0, 1 };

        [TestMethod]
        public void Constructor_ValidDigits_CreatesMyNumberValue()
        {
            // Act
            var myNumber = new MyNumberValue(_validMyNumber);

            // Assert
            Assert.IsTrue(myNumber.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, myNumber.Digits);
        }

        [TestMethod]
        public void Constructor_InvalidDigits_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => new MyNumberValue(_invalidMyNumber));
        }

        [TestMethod]
        public void Constructor_NullDigits_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => new MyNumberValue(null));
        }

        [TestMethod]
        public void Constructor_WrongLength_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => new MyNumberValue(new int[10]));
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => new MyNumberValue(new int[13]));
        }

        [TestMethod]
        public void Constructor_IndividualDigits_CreatesMyNumberValue()
        {
            // Act
            var myNumber = new MyNumberValue(6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0, 0);

            // Assert
            Assert.IsTrue(myNumber.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, myNumber.Digits);
        }

        [TestMethod]
        public void FromFirstElevenDigits_ValidDigits_CreatesMyNumberValue()
        {
            // Arrange
            var firstEleven = new int[] { 6, 1, 4, 1, 0, 6, 5, 2, 6, 0, 0 };

            // Act
            var myNumber = MyNumberValue.FromFirstElevenDigits(firstEleven);

            // Assert
            Assert.IsTrue(myNumber.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, myNumber.Digits);
        }

        [TestMethod]
        public void FromFirstElevenDigits_InvalidLength_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => 
                MyNumberValue.FromFirstElevenDigits(new int[10]));
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => 
                MyNumberValue.FromFirstElevenDigits(null));
        }

        [TestMethod]
        public void TryParse_ValidString_ReturnsTrue()
        {
            // Act
            var success = MyNumberValue.TryParse("614106526000", out var result);

            // Assert
            Assert.IsTrue(success);
            Assert.IsTrue(result.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, result.Digits);
        }

        [TestMethod]
        public void TryParse_ValidStringWithSeparators_ReturnsTrue()
        {
            // Arrange
            var testCases = new[]
            {
                "6141-0652-6000",
                "6141 0652 6000",
                "6141_0652_6000"
            };

            foreach (var testCase in testCases)
            {
                // Act
                var success = MyNumberValue.TryParse(testCase, out var result);

                // Assert
                Assert.IsTrue(success, $"Failed to parse: {testCase}");
                Assert.IsTrue(result.IsInitialized);
                CollectionAssert.AreEqual(_validMyNumber, result.Digits);
            }
        }

        [TestMethod]
        public void TryParse_InvalidString_ReturnsFalse()
        {
            // Arrange
            var invalidCases = new[]
            {
                null,
                "",
                "abc",
                "12345678901",  // too short
                "1234567890123", // too long
                "614106526001", // invalid check digit
                "61a106526000"  // non-digit character
            };

            foreach (var invalidCase in invalidCases)
            {
                // Act
                var success = MyNumberValue.TryParse(invalidCase, out var result);

                // Assert
                Assert.IsFalse(success, $"Should have failed to parse: {invalidCase}");
                Assert.IsFalse(result.IsInitialized);
            }
        }

        [TestMethod]
        public void Parse_ValidString_ReturnsMyNumberValue()
        {
            // Act
            var result = MyNumberValue.Parse("614106526000");

            // Assert
            Assert.IsTrue(result.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, result.Digits);
        }

        [TestMethod]
        public void Parse_InvalidString_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => MyNumberValue.Parse("invalid"));
        }

        [TestMethod]
        public void GenerateRandom_Always_ReturnsValidMyNumberValue()
        {
            // Act
            for (int i = 0; i < 100; i++)
            {
                var random = MyNumberValue.GenerateRandom();

                // Assert
                Assert.IsTrue(random.IsInitialized);
                Assert.IsTrue(MyNumber.VerifyNumber(random.Digits));
            }
        }

        [TestMethod]
        public void ToString_DefaultFormat_ReturnsPlainDigits()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act
            var result = myNumber.ToString();

            // Assert
            Assert.AreEqual("614106526000", result);
        }

        [TestMethod]
        public void ToString_VariousFormats_ReturnsCorrectFormat()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act & Assert
            Assert.AreEqual("614106526000", myNumber.ToString("N"));
            Assert.AreEqual("6141 0652 6000", myNumber.ToString("S"));
            Assert.AreEqual("6141-0652-6000", myNumber.ToString("H"));
            Assert.AreEqual("6141-0652-600-0", myNumber.ToString("G"));
        }

        [TestMethod]
        public void ToString_InvalidFormat_ThrowsException()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act & Assert
            Assert.ThrowsException<FormatException>(() => myNumber.ToString("X"));
        }

        [TestMethod]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var myNumber1 = new MyNumberValue(_validMyNumber);
            var myNumber2 = new MyNumberValue(_validMyNumber);

            // Act & Assert
            Assert.IsTrue(myNumber1.Equals(myNumber2));
            Assert.IsTrue(myNumber1 == myNumber2);
            Assert.IsFalse(myNumber1 != myNumber2);
        }

        [TestMethod]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var myNumber1 = new MyNumberValue(_validMyNumber);
            var myNumber2 = MyNumberValue.GenerateRandom();

            // Act & Assert
            if (!myNumber1.Equals(myNumber2)) // They might be equal by chance
            {
                Assert.IsFalse(myNumber1.Equals(myNumber2));
                Assert.IsFalse(myNumber1 == myNumber2);
                Assert.IsTrue(myNumber1 != myNumber2);
            }
        }

        [TestMethod]
        public void Equals_UninitializedValues_ReturnsTrue()
        {
            // Arrange
            var myNumber1 = new MyNumberValue();
            var myNumber2 = new MyNumberValue();

            // Act & Assert
            Assert.IsTrue(myNumber1.Equals(myNumber2));
            Assert.IsTrue(myNumber1 == myNumber2);
        }

        [TestMethod]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Arrange
            var myNumber1 = new MyNumberValue(_validMyNumber);
            var myNumber2 = new MyNumberValue(_validMyNumber);

            // Act & Assert
            Assert.AreEqual(myNumber1.GetHashCode(), myNumber2.GetHashCode());
        }

        [TestMethod]
        public void ImplicitConversion_ToIntArray_ReturnsDigits()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act
            int[] digits = myNumber;

            // Assert
            CollectionAssert.AreEqual(_validMyNumber, digits);
        }

        [TestMethod]
        public void ImplicitConversion_ToString_ReturnsString()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act
            string str = myNumber;

            // Assert
            Assert.AreEqual("614106526000", str);
        }

        [TestMethod]
        public void ExplicitConversion_FromIntArray_CreatesMyNumberValue()
        {
            // Act
            var myNumber = (MyNumberValue)_validMyNumber;

            // Assert
            Assert.IsTrue(myNumber.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, myNumber.Digits);
        }

        [TestMethod]
        public void ExplicitConversion_FromString_CreatesMyNumberValue()
        {
            // Act
            var myNumber = (MyNumberValue)"614106526000";

            // Assert
            Assert.IsTrue(myNumber.IsInitialized);
            CollectionAssert.AreEqual(_validMyNumber, myNumber.Digits);
        }

        [TestMethod]
        public void ExplicitConversion_FromInvalidIntArray_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<MyNumber.MyNumberMalformedException>(() => (MyNumberValue)_invalidMyNumber);
        }

        [TestMethod]
        public void ExplicitConversion_FromInvalidString_ThrowsException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => (MyNumberValue)"invalid");
        }

        [TestMethod]
        public void Digits_PropertyAccess_ReturnsCopy()
        {
            // Arrange
            var myNumber = new MyNumberValue(_validMyNumber);

            // Act
            var digits1 = myNumber.Digits;
            var digits2 = myNumber.Digits;

            // Assert
            CollectionAssert.AreEqual(digits1, digits2);
            Assert.AreNotSame(digits1, digits2); // Should be different instances
        }

        [TestMethod]
        public void Digits_UninitializedValue_ThrowsException()
        {
            // Arrange
            var myNumber = new MyNumberValue();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => myNumber.Digits);
        }

        [TestMethod]
        public void ToString_UninitializedValue_ThrowsException()
        {
            // Arrange
            var myNumber = new MyNumberValue();

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => myNumber.ToString());
        }
    }
}