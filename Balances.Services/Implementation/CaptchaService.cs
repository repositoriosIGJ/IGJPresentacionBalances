using Balances.DTO;
using Balances.Services.Contract;
using Balances.Web.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Balances.Services.Implementation
{
    public class CaptchaService : ICaptchaService
    {
        private readonly HttpClient _httpClient;
        public const string SecretKey = "0x4AAAAAAAgjjsKciWEKJ6Avofmw5LEeagA";

        public CaptchaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<ResponseDTO<CaptchaResponse>> Validate(tokenRequest token)
        {
            ResponseDTO<CaptchaResponse> respuesta = new();
            respuesta.IsSuccess = false;
            try
            {
                CloudflareTurnstileVerifyRequestModel requestModel = new(SecretKey, token.token);

                var content = new StringContent(
           JsonSerializer.Serialize(requestModel),
           Encoding.UTF8,
           "application/json"
       );

                var response = await _httpClient.PostAsJsonAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var captchaResponse = JsonSerializer.Deserialize<CaptchaResponse>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    respuesta.IsSuccess = true;
                    respuesta.Result = captchaResponse;
                }

                return respuesta;

            }
            catch (HttpRequestException ex)
            {
                respuesta.Message = ($"Error en la solicitud HTTP: {ex.Message}");
                respuesta.IsSuccess = false;

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Message = ($"Error inesperado: {ex.Message}");
                respuesta.IsSuccess = false;

                return respuesta;
            }
        }

        public record class CloudflareTurnstileVerifyRequestModel(
    [property: JsonPropertyName("secret")] string SecretKey,
    [property: JsonPropertyName("response")] string Token);


    }
}

public class CaptchaResponse
{
    public bool success { get; set; }
    public string challenge_ts { get; set; }
    public string hostname { get; set; }
}