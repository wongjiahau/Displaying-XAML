using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeDisplayer {
    public static class Helper {
        public static string DownloadFile(string sourceUrl) //https://gist.github.com/nboubakr/7812375
        {
            long existLen = 0;
            var httpReq = (HttpWebRequest) WebRequest.Create(sourceUrl);
            httpReq.AddRange((int) existLen);
            var httpRes = (HttpWebResponse) httpReq.GetResponse();
            var responseStream = httpRes.GetResponseStream();
            if (responseStream == null) return "Fail to fetch file";
            var streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();

        }
    }
}