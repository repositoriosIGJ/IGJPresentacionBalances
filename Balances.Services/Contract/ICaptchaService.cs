using Balances.DTO;
using Balances.Web.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balances.Services.Contract
{
    public interface ICaptchaService
    {
        Task<ResponseDTO<CaptchaResponse>> Validate(tokenRequest token);
    }
}
