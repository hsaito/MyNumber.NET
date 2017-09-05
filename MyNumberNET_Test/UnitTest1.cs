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
            var subject = "123456789010".ToCharArray();
            var value = Array.ConvertAll(subject, c => (int) char.GetNumericValue(c));

            for (var i = 0; i < 11; i++)
            {
                value[11] = i;
                if (i == 8 && MyNumber.VerifyNumber(value))
                {
                    // This is OK
                }
                else if (MyNumber.VerifyNumber(value))
                {
                    throw new Exception();
                }
            }
        }
    }
}