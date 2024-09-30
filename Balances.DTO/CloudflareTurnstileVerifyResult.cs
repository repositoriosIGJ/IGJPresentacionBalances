using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Balances.DTO
{
    public record class CloudflareTurnstileVerifyResult(
     // https://developers.cloudflare.com/turnstile/get-started/server-side-validation/
     [property: JsonPropertyName("success")] bool Success,
     [property: JsonPropertyName("error-codes")] string[] ErrorCodes,
     [property: JsonPropertyName("challenge_ts")] DateTimeOffset On,
     [property: JsonPropertyName("hostname")] string Hostname
     );

    
}
