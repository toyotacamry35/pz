using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Monitoring.Contract
{
    public class MonitoringApi
    {
        private readonly string _address;
        private readonly HttpClient _httpClient;

        public MonitoringApi(string address)
        {
            _address = address;
            _httpClient = new HttpClient();
        }
        
        public async Task<TargetProcessRegisterModel.Result> Register(TargetProcessRegisterModel registerModel)
        {
            var response = await _httpClient.PostAsync($"{_address}/api/observer/register", new StringContent(
                JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TargetProcessRegisterModel.Result>(await response.Content.ReadAsStringAsync());  
        }
    }
}