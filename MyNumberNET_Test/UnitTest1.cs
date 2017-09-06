using System;
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
            var subjectarray = new[]
            {
                "614106526000", "510136263801", "060122228102",
                "362473502703", "467430101604", "763727588705",
                "734220726006", "450817747707", "207304711608",
                "407508284309"
            };

            foreach (var item in subjectarray)
            {
                var subject = item.ToCharArray();
                var value = Array.ConvertAll(subject, c => (int) char.GetNumericValue(c));
                if(!MyNumber.VerifyNumber(value))
                    throw new Exception("Validation failed.");
            }
        }
    }
}