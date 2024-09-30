using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Balances.DTO
{
    public record class CloudflareTurnstileVerifyRequestModel(
     // https://developers.cloudflare.com/turnstile/get-started/server-side-validation
     [property: JsonPropertyName("secret")] string SecretKey,
     [property: JsonPropertyName("response")] string Token,
     [property: JsonPropertyName("remoteip")] string? UserIpAddress,
     [property: JsonPropertyName("idempotency_key")] string? IdempotencyKey);
}
