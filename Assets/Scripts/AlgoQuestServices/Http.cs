using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace AlgoQuestServices
{
    public struct HttpResponse<T>
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public HttpRequestException Exception { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    
    public static class Http
    {
        private const string BaseUrl = "http://localhost:3000/api";
        private const string ApiKey =
            "c1594cd0a9ad6a208b605cd30f90e4bb9576095b9397e13f22045666d040eea84c5db3b888218396f54f98bbe6ba9ae9";
        
        private static readonly HttpClient Client = new()
        {
            DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", ApiKey) }
        };

        public static string SessionId { get; set; }

        public static async Task<HttpResponse<T>> Get<T>(string uri)
        {
            var response = await Client.GetAsync($"{BaseUrl}/{uri}");

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException exception)
            {
                return CreateErrorResponse<T>(response, exception);
            }
            
            return await CreateSuccessResponse<T>(response);
        }
        
        public static async Task<HttpResponse<T>> Post<T, TV>(string uri, TV data)
        {
            string json;

            try
            {
                json = JsonConvert.SerializeObject(data);
            }
            catch (JsonException)
            {
                return new HttpResponse<T>
                {
                    Success = false,
                    Message = "Unable to serialize request body"
                };
            }
                
            var body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await Client.PostAsync($"{BaseUrl}/{uri}", body);
            
            try
            {
                response.EnsureSuccessStatusCode();
            } 
            catch (HttpRequestException exception)
            {
                return CreateErrorResponse<T>(response, exception);
            }

            return await CreateSuccessResponse<T>(response);
        }
        
        private static async Task<HttpResponse<T>> CreateSuccessResponse<T>(HttpResponseMessage response)
        {
            T data;

            try
            {
                data = response.StatusCode != HttpStatusCode.NoContent
                    ? await ExtractContent<T>(response.Content)
                    : default;
            }
            catch (JsonException)
            {
                return new HttpResponse<T>
                {
                    Success = false,
                    Message = "Unable to parse JSON response"
                };
            }
            
            return new HttpResponse<T>
            {
                Success = true,
                StatusCode = response.StatusCode,
                Data = data
            };
        }
        
        private static HttpResponse<T> CreateErrorResponse<T>(HttpResponseMessage response, HttpRequestException exception)
        {
            return new HttpResponse<T>
            {
                Success = false,
                StatusCode = response.StatusCode,
                Exception = exception,
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Bad Request",
                    HttpStatusCode.Unauthorized => "Unauthorized",
                    HttpStatusCode.InternalServerError => "Internal Server Error",
                    _ => "Unknown error"
                }
            };
        }
        
        private static async Task<T> ExtractContent<T>(HttpContent httpContent)
        {
            if (httpContent == null) return default;
            
            var data = await httpContent.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
