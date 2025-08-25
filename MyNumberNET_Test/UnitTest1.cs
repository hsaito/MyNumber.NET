using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNumberNET;

namespace MyNumberNET_Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GenerateRandom()
        {
            var n = new MyNumber();

            for (var i = 0; i < 100; i++)
            {
                var rSeq = n.GenerateRandomNumber();
                var check = MyNumber.VerifyNumber(rSeq);
                if (check == false)
                    throw new Exception();
            }
        }

        [TestMethod]
        public void SampleTest()
        {
            // Some randomly generated array of My Numbers
            var subjectArray = new[]
            {
                "614106526000", "510136263801", "060122228102",
                "362473502703", "467430101604", "763727588705",
                "734220726006", "450817747707", "207304711608",
                "407508284309"
            };

            if (subjectArray.Select(item => item.ToCharArray())
                .Select(subject => Array.ConvertAll(subject, c => (int) char.GetNumericValue(c)))
                .Any(value => !MyNumber.VerifyNumber(value))) throw new Exception("Validation failed.");
        }

        [TestMethod]
        public void InvalidTest()
        {
            // Some randomly generated array of My Numbers (Invalid ones)
            var subjectArray = new[]
            {
                "614106526006", "510136263805", "060122228103",
                "362473502704", "467430101602", "763727588707",
                "734220726004", "450817747706", "207304711604",
                "407508284302"
            };

            if (subjectArray.Select(item => item.ToCharArray())
                .Select(subject => Array.ConvertAll(subject, c => (int) char.GetNumericValue(c)))
                .Any(MyNumber.VerifyNumber)) throw new Exception("Validation failed.");
        }

        [TestMethod]
        public void VerifyNumber_NullInput_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.VerifyNumber(null));
        }

        [TestMethod]
        public void VerifyNumber_WrongLength_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.VerifyNumber(new int[10]));
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.VerifyNumber(new int[13]));
        }

        [TestMethod]
        public void VerifyNumber_InvalidDigits_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.VerifyNumber(new int[] { 1,2,3,4,5,6,7,8,9,10,11,12 }));
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.VerifyNumber(new int[] { -1,2,3,4,5,6,7,8,9,0,1,2 }));
        }

        [TestMethod]
        public void CalculateCheckDigits_NullInput_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.CalculateCheckDigits(null));
        }

        [TestMethod]
        public void CalculateCheckDigits_WrongLength_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.CalculateCheckDigits(new int[10]));
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.CalculateCheckDigits(new int[12]));
        }

        [TestMethod]
        public void CalculateCheckDigits_InvalidDigits_ThrowsException()
        {
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.CalculateCheckDigits(new int[] { 1,2,3,4,5,6,7,8,9,10,11 }));
            Assert.ThrowsExactly<MyNumber.MyNumberMalformedException>(() => MyNumber.CalculateCheckDigits(new int[] { -1,2,3,4,5,6,7,8,9,0,1 }));
        }
    }
}