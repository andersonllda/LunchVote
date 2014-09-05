using System;

namespace HMV.Core.Framework.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        ///  formata para dois numeros de casa decimal sem arredondar.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToTwoCaseFormat(this double value)
        {
            double result = 0;

            //if decimal
            if (Math.Round(value, 2) != value)
                result = Math.Floor(value * 100) / 100;
            else
                result = value;

            return result;
        }
    }
}

