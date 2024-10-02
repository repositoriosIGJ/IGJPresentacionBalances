using Balances.Web.Services.Contracts;
using static Balances.Web.Services.Implementation.CaptchaClientService;
using System.Net.Http.Json;
using System.Text;
using Balances.DTO;

namespace Balances.Web.Services.Implementation
{
    public class CaptchaClientService : ICaptchaClientService
    {

        private readonly HttpClient _httpClient;

        public CaptchaClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<bool> ValidatarCaptcha(tokenRequest token)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/captcha/validate", token);

                if (response.IsSuccessStatusCode)
                {
                    var captchaResponse = await response.Content.ReadFromJsonAsync<ResponseDTO<CaptchaResponse>>();
                    return captchaResponse.Result.success;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }

        }
    }
    public class CaptchaResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
    }

    public class tokenRequest
    {
        public string token { get; set; }
    }
}

