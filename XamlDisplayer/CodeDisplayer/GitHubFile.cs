using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeDisplayer {
    public class GitHubFile {
        private readonly string _ownerName;
        private readonly string _repositoryName;
        private readonly string _branchName;
        private readonly string _folderPath;
        private readonly string _fileName;

        public GitHubFile(string ownerName, string repositoryName, string branchName, string folderPath, string fileName) {
            _ownerName = ownerName;
            _repositoryName = repositoryName;
            _branchName = branchName;
            _folderPath = folderPath;
            _fileName = fileName;
        }

        public string GetRaw() {
            string fullUrl = BuildFullUrl();
            return DownloadFile(fullUrl);
        }

        public XmlDocument GetXmlDocument() {
            var result = new XmlDocument();
            result.LoadXml(GetRaw());
            return result;
        }

        private string BuildFullUrl() {
            return
                $@"https://raw.githubusercontent.com/{_ownerName}/{_repositoryName}/{_branchName}/{_folderPath}/{_fileName}";
        }

        private static string DownloadFile(string sourceUrl) //https://gist.github.com/nboubakr/7812375
        {
            
            long existLen = 0;
            var httpReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            httpReq.AddRange((int)existLen);
            var httpRes = (HttpWebResponse)httpReq.GetResponse();
            var responseStream = httpRes.GetResponseStream();
            if (responseStream == null) return "Fail to fetch file";
            var streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }
    }
}
