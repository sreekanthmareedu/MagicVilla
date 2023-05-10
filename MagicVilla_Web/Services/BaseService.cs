using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get ; set ; }
        public IHttpClientFactory httpClient { get; set ; }



        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                    Encoding.UTF8, "application/json");

                }
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;


                }

                HttpResponseMessage apiResponse = null;

                if (!string.IsNullOrEmpty(apiRequest.Token))
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiRequest.Token);  
                }


                apiResponse = await client.SendAsync(message);

                

                var apiContent = await apiResponse.Content.ReadAsStringAsync();


                //   var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                try
                {
                    APIResponse aPIResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if (apiResponse.StatusCode == HttpStatusCode.BadRequest || apiResponse.StatusCode == HttpStatusCode.NotFound)

                    {
                        aPIResponse.StatusCode = HttpStatusCode.BadRequest;
                        aPIResponse.isSuccess = false;
                        var res = JsonConvert.SerializeObject(aPIResponse);
                        var resobj = JsonConvert.DeserializeObject<T>(res);
                        return resobj;

                    }
                }
                catch (Exception e)
                {
                    var exceptionresponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionresponse;

                }

                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);

                return APIResponse;

            }
            catch(Exception e)
            {
                var dto = new APIResponse
                {
                    ErrorMessage = new List<string> { Convert.ToString(e.Message) },
                    isSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
