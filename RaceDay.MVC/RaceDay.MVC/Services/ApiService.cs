using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace RaceDay.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7249/api/";

            // Get token from cookie and attach it to requests
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Generic GET method
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }

        // Generic POST method
        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
        }

        // Generic PUT method
        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync($"{_baseUrl}{endpoint}", content);
        }

        // Generic DELETE method
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _httpClient.DeleteAsync($"{_baseUrl}{endpoint}");
        }
    }
}