using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService
{
    public class HttpUtility
    {

        public HttpUtility(IConfiguration _configuration)
        {
            configuration = _configuration;

        }
        public HttpUtility()
        {
        }

        public IConfiguration configuration { get; }

        public async Task<string> HttpGet(string url, Dictionary<string, string> parameters = null)
        {
            string apiResponse = string.Empty;
            parameters = (parameters == null) ? new Dictionary<string, string>() : parameters;
            using (var httpClient = new HttpClient())
            {
                if (parameters.Count > 0)
                {
                    foreach (var param in parameters)
                    {
                        url += string.Format("?{0}={1}", param.Key, param.Value);
                    }
                }

                using (var response = await httpClient.GetAsync(url))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return apiResponse;
        }

        public async Task<string> HttpPost(string url, string body)
        {
            string apiResponse = string.Empty;
            using (var httpClient = new HttpClient())
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(body);

                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var response = await httpClient.PostAsync(url, byteContent))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return apiResponse;
        }

        public async Task<string> HttpPost(string url, Dictionary<string, string> parameters)
        {
            string apiResponse = string.Empty;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(parameters)))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return apiResponse;
        }

        public async Task<bool> HttpPut(string url, string body)
        {
            bool status = false;
            HttpClient httpClient = new HttpClient();
            var requestContent = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            { status = true; }
            else { status = false; }
            httpClient.Dispose();
            return status;

        }
    }
}
