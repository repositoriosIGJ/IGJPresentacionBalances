using Microsoft.AspNetCore.Mvc;
using Balances.Services.Contract;
using Balances.Web.Services.Implementation;

namespace Balances.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : Controller
    {
         private readonly ICaptchaService _captchaService;

    public CaptchaController(ICaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCaptcha([FromBody]tokenRequest token)
        {
            
            var rsp = await _captchaService.Validate(token);
                return Ok(rsp);
          
        }


   
    }
}
