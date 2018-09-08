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
    }
}