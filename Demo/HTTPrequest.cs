using System;
using System.Threading.Tasks;
using System.Xml;
using Windows.Web.Http;

namespace Demo
{
	public class HTTPRequest
	{
        private static HTTPRequest _instance = null;
        HttpClient _client = null;

        const string HTTP_PREFIX = "http://";
        const string HTTP_PORT = ":8080";
        const string HTTP_REQURL_PARAM = "/e-FilingBox/boxnwGetSecuritySetting";
        string strEngineVersion = string.Empty;

        public static HTTPRequest GetInstance()
        {
            if (_instance == null)
            {
                _instance = new HTTPRequest();
            }
            return _instance;
        }

        HTTPRequest()
        {
            _client = new HttpClient();
        }

        public string EngineVersion
        {
            get => strEngineVersion;
        }

        public async Task FetchEngineVersion(string strIPAddress)
		{   
            try
            {
                string strResponseMsg = string.Empty;
                string stURL = HTTP_PREFIX + strIPAddress + HTTP_PORT + HTTP_REQURL_PARAM;
                Uri reqURI = new Uri(stURL);

                HttpResponseMessage response = new HttpResponseMessage();
                response = await _client.GetAsync(reqURI);
                response.EnsureSuccessStatusCode();
                strResponseMsg = await response.Content.ReadAsStringAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strResponseMsg);
                XmlNodeList nodeLst = doc.GetElementsByTagName("Version");
                string strTemp = nodeLst[0].FirstChild.InnerText;
                string[] stTmp = strTemp.Split(' ');
                strEngineVersion = stTmp[stTmp.Length - 1];
            }
            catch (Exception ex)
            {

            }
		}
	}
}
