using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace HMV.Core.Framework.Helper
{
    public static class ZipHelper
    {
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream str = new MemoryStream(data);
            str.Seek(0, SeekOrigin.Begin);

            byte[] output = null;
            //string st = new ASCIIEncoding().GetString(data);
            //output = Encoding.ASCII.GetBytes(st);

            ZipEntry entry;


            if (data != null)
            {
                try
                {

                    using (ZipInputStream s = new ZipInputStream(str))
                    {
                        entry = s.GetNextEntry();
                        //int size = 1900680;
                        int size = 3900680;
                        byte[] buffer = new byte[size];

                        size = s.Read(buffer, 0, size);

                        output = buffer;
                    }

                }
                catch
                {
                    output = null;
                }
            }


            return output;
        }

    }
}
