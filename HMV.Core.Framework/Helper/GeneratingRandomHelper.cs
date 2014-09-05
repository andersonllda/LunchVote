using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HMV.Core.Framework.Helper
{
    public static class GeneratingRandomHelper
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static string Random(int size, bool lowerCase)
        {
            string random = Guid.NewGuid().ToString().Substring(0,size);

            if (lowerCase)
                return random;
            return random.ToUpper();


           /* StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                Random random = new Random();
                int codigo = Convert.ToInt32(random.Next(48, 122).ToString());
                if ((codigo >= 48 && codigo <= 57) || (codigo >= 97 && codigo <= 122))
                {
                    string _char = ((char)codigo).ToString();
                    if (!builder.ToString().Contains(_char))
                    {
                        builder.Append(_char);
                    }
                    else
                    {
                        i--;
                    }
                }
                else
                {
                    i--;
                }
            }

            if (lowerCase)
                return builder.ToString();
            return builder.ToString().ToUpper();*/
        }
        public static string RandomNew(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                Random random = new Random();
                //int codigo = Convert.ToInt32(random.Next(48, 122).ToString());
                //int codigo1 = random.Next(48, 57);
                int codigo2 = random.Next(97, 122);
                //if ((codigo >= 48 && codigo <= 57) || (codigo >= 97 && codigo <= 122))
                {
                    //string _char1 = ((char)codigo1).ToString();
                    string _char2 = ((char)codigo2).ToString();
                    if (!builder.ToString().Contains(_char2))
                    {
                        builder.Append(_char2);
                    }
                    else
                    {
                        i--;
                    }
                }
                //else
                //{
                //    i--;
                //}
            }

            if (lowerCase)
                return builder.ToString();
            return builder.ToString().ToUpper();
        }
    }
}
