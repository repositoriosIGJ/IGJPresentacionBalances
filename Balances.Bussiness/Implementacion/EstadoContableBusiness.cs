﻿using AutoMapper;
using Balances.Bussiness.Contrato;
using Balances.DTO;
using Balances.Model;
using Balances.Services.Contract;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Balances.Bussiness.Implementacion
{
    public class EstadoContableBusiness : IEstadoContableBusiness
    {

        private readonly ISessionService _sessionService;
        private readonly IBalanceBusiness _balanceBusiness;
        private readonly IMapper _mapper;
        private readonly ILogger<EstadoContableBusiness> _logger;


        public EstadoContableBusiness(ISessionService sessionService,
                                      IBalanceBusiness balanceBusiness,
                                      IMapper mapper,
                                      ILogger<EstadoContableBusiness> logger)
        {
            _sessionService = sessionService;
            _balanceBusiness = balanceBusiness;
            _mapper = mapper;
            _logger = logger;
        }

        public ResponseDTO<BalanceDto> Delete(RubroPatrimonioNetoDto modelo)
        {

            var resultadoDto = new ResponseDTO<BalanceDto>();
            resultadoDto.IsSuccess = false;

            var rubroSerializado = JsonConvert.SerializeObject(modelo);
            try
            {
                // RECUPERO EL BALANCE ACTUAL
                var bDto = _balanceBusiness.BalanceActual;

                //BUSCO EL OTRO RUBRO A BORRAR
                var rubro = bDto.EstadoContable.otrosRubros.FirstOrDefault(p => p.Codigo == modelo.codigo);

                if (rubro != null)
                {
                    bDto.EstadoContable.otrosRubros.Remove(rubro);
                }

                //ACTUALIZO LA DB
                var rst = _balanceBusiness.Update(bDto);

                resultadoDto = rst;

                _logger.LogInformation($"EstadoContableBusiness.Insert rubro  --> {rubroSerializado}");
            }
            catch (Exception ex)
            {
                resultadoDto.Message = ex.Message;
                _logger.LogError($"EstadoContableBusiness.Delete rubro: \n {ex}");

            }
            return resultadoDto;

        }

        public ResponseDTO<BalanceDto> Insert(EstadoContableDto modelo)
        {
            ResponseDTO<BalanceDto> respuesta = new ResponseDTO<BalanceDto>();
            respuesta.IsSuccess = false;
            var EECCSerializado = JsonConvert.SerializeObject(modelo);
            try
            {
                var id = _sessionService.GetBalanceId();
                var resultadoDto = _balanceBusiness.GetById(id);

                if (resultadoDto.IsSuccess)
                {
                    var balanceDto = resultadoDto.Result;


                    balanceDto.EstadoContable = _mapper.Map<EstadoContable>(modelo);

                    if (balanceDto.EstadoContable.otrosRubros == null)
                        balanceDto.EstadoContable.otrosRubros = new List<RubroPatrimonioNeto>();


                    foreach (var rubro in balanceDto.EstadoContable.otrosRubros.ToList())
                    {

                        rubro.Codigo = Guid.NewGuid().ToString();
                        balanceDto.EstadoContable.otrosRubros.Add(rubro);
                        //balanceDto.EstadoContable.otrosRubros.AddRange(rubro);
                    }


                    var rsp = _balanceBusiness.Update(balanceDto);

                    _logger.LogInformation($"EstadoContableBusiness.Insert EECC --> {EECCSerializado} ");
                    respuesta = rsp;
                }

                //if (resultadoDto.IsSuccess)
                //{
                //    var balanceDto = resultadoDto.Result;

                //    /*ACTUALIZAR BALANCE CON EL DTO*/


                //    if (balanceDto.EstadoContable.otrosRubros == null)
                //        balanceDto.EstadoContable.otrosRubros = new List<RubroPatrimonioNeto>();

                //    foreach (var rubro in balanceDto.EstadoContable.otrosRubros.ToList())
                //    {
                //        rubro.Codigo = Guid.NewGuid().ToString();
                //        balanceDto.EstadoContable.otrosRubros.Add(rubro);
                //    }

                //    var rsp = _balanceBusiness.Update(balanceDto);

                //    respuesta = rsp;
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"EstadoContableBusiness.Insert: \n {ex}");
                respuesta.Message = ex.Message;
            }

            return respuesta;

        }


        public ResponseDTO<BalanceDto> Insert(RubroPatrimonioNetoDto modelo)
        {
            ResponseDTO<BalanceDto> respuesta = new ResponseDTO<BalanceDto>();
            respuesta.IsSuccess = false;
            var rubroSerializado = JsonConvert.SerializeObject(modelo);
            try
            {
                var id = _sessionService.GetBalanceId();
                var resultadoDto = _balanceBusiness.GetById(id);

                if (resultadoDto.IsSuccess)
                {
                    var balanceDto = resultadoDto.Result;


                    balanceDto.EstadoContable = new EstadoContable();

                    if (balanceDto.EstadoContable.otrosRubros == null)
                        balanceDto.EstadoContable.otrosRubros = new List<RubroPatrimonioNeto>();


                    /*foreach (var rubro in balanceDto.EstadoContable.otrosRubros.ToList())
                    {

                        rubro.Codigo = Guid.NewGuid().ToString();
                        balanceDto.EstadoContable.otrosRubros.Add(rubro);
                        //balanceDto.EstadoContable.otrosRubros.AddRange(rubro);
                    }*/
                    var rubro = _mapper.Map<RubroPatrimonioNeto>(modelo);
                    rubro.Codigo = Guid.NewGuid().ToString();
                    balanceDto.EstadoContable.otrosRubros.Add(rubro);
                    var rsp = _balanceBusiness.Update(balanceDto);
                    _logger.LogInformation($"EstadoContableBusiness.Insert rubro --> {rubroSerializado} ");
                    respuesta = rsp;
                }

          
            }
            catch (Exception ex)
            {
                respuesta.Message = ex.Message;
                _logger.LogError($"EstadoContableBusiness.Insert modelo RubroPatrimonioNeto: \n {ex}");
            }

            return respuesta;

        }

       
    }
}
