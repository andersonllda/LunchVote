using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HMV.Core.Framework.Helper
{
    public static class DownloadHelper
    {
        public static byte[] Download(string uri)
        {
            WebClient client = new WebClient();
            return client.DownloadData(uri);
        }
    }
}
