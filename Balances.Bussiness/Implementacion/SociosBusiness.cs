﻿using Balances.Bussiness.Contrato;
using Balances.DTO;
using Balances.Services.Contract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Balances.Bussiness.Implementacion
{
    public class SociosBusiness : ISociosBusiness

    {
        private readonly IBalanceBusiness _balanceBusiness;
        private readonly ISessionService _sessionService;

        private readonly ILogger<SociosBusiness> _logger;

        public SociosBusiness(IBalanceBusiness balanceBusiness,
                               ILogger<SociosBusiness> logger,
                               ISessionService sessionService
                              )
        {
            _balanceBusiness = balanceBusiness;
            _logger = logger;
            _sessionService = sessionService;
        }

        public ResponseDTO<BalanceDto> InsertPersonaJuriridica(PersonaJuridicaDto modelo)
        {
            ResponseDTO<BalanceDto> respuesta = new ResponseDTO<BalanceDto>();
            var pjSerializada = JsonConvert.SerializeObject(modelo);

            try
            {
                var id = _sessionService.GetBalanceId(modelo.SesionId);

                var bDto = _balanceBusiness.GetById(id);


                modelo.Id = Guid.NewGuid().ToString();


                if (bDto.Result.Socios == null) bDto.Result.Socios = new SociosDto();

                if (bDto.Result.Socios.PersonasJuridicas == null)
                    bDto.Result.Socios.PersonasJuridicas = new List<PersonaJuridicaDto>();

                bDto.Result.Socios.PersonasJuridicas.Add(modelo);




                _balanceBusiness.Update(bDto.Result);

                respuesta.IsSuccess = true;
                respuesta.Result = bDto.Result;
                respuesta.Message = $"Persona juridica guardada correctamente";
                _logger.LogInformation($"SociosBusiness.InsertPersonaJuriridica : ---> {pjSerializada}");

            }
            catch (Exception ex)
            {

                respuesta.Message = ex.Message;
                _logger.LogError($"SociosBusiness.InsertPersonaJuriridica: \n {ex}");
            }

            return respuesta;
        }

        public ResponseDTO<BalanceDto> InsertPersonaHumana(PersonaHumanaDto modelo)
        {
            ResponseDTO<BalanceDto> respuesta = new ResponseDTO<BalanceDto>();
            var pjSerializada = JsonConvert.SerializeObject(modelo);

            try
            {
                var id = _sessionService.GetBalanceId(modelo.SesionId);
                var bDto = _balanceBusiness.GetById(id);


                modelo.Id = Guid.NewGuid().ToString();

                if (bDto.Result.Socios == null) bDto.Result.Socios = new SociosDto();

                if (bDto.Result.Socios.PersonasHumanas == null)
                    bDto.Result.Socios.PersonasHumanas = new List<PersonaHumanaDto>();

                bDto.Result.Socios.PersonasHumanas.Add(modelo);




                _balanceBusiness.Update(bDto.Result);

                respuesta.IsSuccess = true;
                respuesta.Result = bDto.Result;
                respuesta.Message = "Persona humana guardada correctamente";
                _logger.LogInformation($"SociosBusiness.InsertPersonaHumana : ---> {pjSerializada}");

            }
            catch (Exception ex)
            {

                respuesta.Message = ex.Message;
                _logger.LogError($"SociosBusiness.InsertPersonaHumana \n {ex}");
            }

            return respuesta;
        }

        public ResponseDTO<BalanceDto> DeletePersonaHumana(PersonaHumanaDto modelo)
        {
            var respuesta = new ResponseDTO<BalanceDto>();
            var pjSerializada = JsonConvert.SerializeObject(modelo);
            respuesta.IsSuccess = false;
            try
            {
                var id = _sessionService.GetBalanceId(modelo.SesionId);
                var bal = _balanceBusiness.GetById(id);




                var humano = bal.Result.Socios.PersonasHumanas.FirstOrDefault(x => x.Id == modelo.Id);

                if (humano != null) bal.Result.Socios.PersonasHumanas.Remove(humano);

                _balanceBusiness.Update(bal.Result);
                respuesta.Result = bal.Result;
                respuesta.IsSuccess = true;
                respuesta.Message = "persona humana borrada correctamente";
                _logger.LogInformation($"SociosBusiness.DeletePersonaHumana :  ---> {pjSerializada}");
            }
            catch (Exception ex)
            {

                respuesta.Message = ex.Message;
                _logger.LogError($"SociosBusiness.DeletePersonaHumana: \n {ex}");
            }

            return respuesta;

        }

        public ResponseDTO<BalanceDto> DeletePersonaJuriridica(PersonaJuridicaDto modelo)
        {
            var respuesta = new ResponseDTO<BalanceDto>();
            respuesta.IsSuccess = false;
            var pjSerializada = JsonConvert.SerializeObject(modelo);
            try
            {
                var id = _sessionService.GetBalanceId(modelo.SesionId);
                var bal = _balanceBusiness.GetById(id);

                var juridica = bal.Result.Socios.PersonasJuridicas.FirstOrDefault(x => x.Id == modelo.Id);

                if (juridica != null) bal.Result.Socios.PersonasJuridicas.Remove(juridica);

                _balanceBusiness.Update(bal.Result);
                respuesta.Result = bal.Result;
                respuesta.IsSuccess = true;
                respuesta.Message = "persona juridica borrada correctamente";
                _logger.LogInformation($"SociosBusiness.DeletePersonaJuriridica : ---> {pjSerializada}");
            }
            catch (Exception ex)
            {

                respuesta.Message = ex.Message;
                _logger.LogError($"SociosBusiness.DeletePersonaJuriridica: \n {ex}");
            }

            return respuesta;
        }
    }
}
