using System;

namespace ObligatorioDa2.Domain.Util
{
    public class GeneradorCodigoRandom
    {
        public static int RandomCode()
        {
            Random generator = new Random();
            int code = generator.Next(100000, 1000000);
            return code;
        }
    }
}
