﻿using Balances.DTO;

namespace Balances.Web.Services.Implementation
{
    public interface IContadorService
    {
        Task<ResponseDTO<BalanceDto>> postContador(ContadorDto contador);

        Task<ResponseDTO<BalanceDto>> getBalance();



    }
}
