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

            for (int i = 0; i < 100; i++)
            {
                var r_seq = n.GenerateRandomNumber();
                var check = n.VerifyNumber(r_seq);
                if(check == false)
                    throw new Exception();
            }
        }

        [TestMethod]
        public void SampleTest()
        {
            var n = new MyNumber();
            var subject = "123456789010".ToCharArray();
            int[] value = Array.ConvertAll(subject, c => (int)Char.GetNumericValue(c));

            for(int i = 0; i < 11; i++)
            {
                value[11] = i;
                if(i == 8 && n.VerifyNumber(value))
                {
                    // This is OK
                }
                else if (n.VerifyNumber(value))
                    throw new Exception();
            }

        }
    }
}
