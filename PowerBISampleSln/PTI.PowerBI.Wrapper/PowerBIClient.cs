using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PTI.PowerBI.Wrapper
{
    public class PowerBIApiResponse<T> where T: class
    {
        public T Data { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
    }
    public class PowerBIClient
    {
        private string AccessToken { get; set; }

        public PowerBIClient(string pAccessToken)
        {
            this.AccessToken = pAccessToken;
        }

        public PowerBIApiResponse<Entities.Reports> GetReports(string pGroupId = null)
        {
            string requestUrl = string.Empty;
            if (!string.IsNullOrEmpty(pGroupId))
            {
                requestUrl = string.Format("https://api.powerbi.com/v1.0/myorg/{0}/reports", pGroupId);
            }
            else
            {
                requestUrl = "https://api.powerbi.com/v1.0/myorg/reports";
            }
            var request = this.CreateHttpRequest(requestUrl, HttpMethod.Get);
            PowerBIApiResponse<Entities.Reports> objReports =
                ExecuteRequest<Entities.Reports>(request);
            return objReports;
        }

        private PowerBIApiResponse<T> ExecuteRequest<T>(HttpWebRequest request)
            where T: class, new()
        {
            PowerBIApiResponse<T> result = new PowerBIApiResponse<T>();
            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();
            result.ResponseCode = response.StatusCode;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                string returnedJson = reader.ReadToEnd();
                result.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(returnedJson);
                reader.Close();
            }
            return result;
        }

        private HttpWebRequest CreateHttpRequest(string requestUrl, HttpMethod method)
        {
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method.Method;
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", string.Format("Bearer {0}", this.AccessToken));
            return request;
        }


    }
}
