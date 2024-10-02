using Balances.Web.Services.Implementation;

namespace Balances.Web.Services.Contracts
{
    public interface ICaptchaClientService
    {
        Task<bool> ValidatarCaptcha(tokenRequest token);
    }
}
