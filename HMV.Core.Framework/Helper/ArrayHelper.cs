using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Helper
{
    public class ArrayHelper
    {
        public static T[] GetMergedArray<T>(T[] originalArray, T[] newArray)
        {
            int startIndexForNewArray = originalArray.Length;
            Array.Resize<T>(ref originalArray, originalArray.Length + newArray.Length);
            newArray.CopyTo(originalArray, startIndexForNewArray);
            return originalArray;
        }


    }
}
