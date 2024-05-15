using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Balances.Web;
using Balances.Web.Shared;
using Blazorise;
using Radzen;
using Radzen.Blazor;
using Balances.DTO;
using Balances.Model;
using Balances.Utilities;
using Balances.Web.Services.Contracts;
using Balances.Web.Services.Implementation;
using Microsoft.AspNetCore.Http;
using BlazorInputFileExtended;
using Microsoft.AspNetCore.Http.Internal;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using Microsoft.Win32;
using Blazorise.Extensions;
using System.Net.Mime;

namespace Balances.Web.Pages
{
    public partial class Archivos
    {

        private string[] tipoDeArchivo =
        {
            "Estado Contable",
            "Acta reuni�n organo",
            "Acta reuni�n administradores",
            "Informe Fiscalizacion",
            "Otro"
        };

        private string? categoria;

        RadzenUpload uploadDD;

        RadzenGrid<ArchivoDTO> grid;

        [Parameter]
        public string? TipoEntidad { get; set; }

        [Parameter]
        public string? balid { get; set; }

        [Parameter]
        public string sesionId { get; set; }

 
        private string msgError = "";
        private string msgErrorTipoArchivo = "";

    
        private List<ArchivoDTO> listArchivo = new List<ArchivoDTO>();
        private ArchivoDTO archivo = new ArchivoDTO();


        protected override async void OnInitialized()
        {
            await Load();
            base.OnInitialized();
        }

        private async Task Load()
        {
            ResponseDTO<BalanceDto> rsp = new();
            sesionId = await sessionStorage.GetItemAsync<string>("SessionId");
            
            try
            {
                if (sesionId == null)
                {
                    var sesionRespuesta = await sesionService.getNewSession();
                    
                    sesionId = sesionRespuesta.Result!;

                    await sessionStorage.SetItemAsync("SessionId", sesionId);
                }
                else
                {
                    var rst = await sesionService.getBalanceId(sesionId);
                    if (rst is not null)
                    {
                        balid = rst;
                        rsp = await balanceService.getBalance(balid);
                        if (rsp.IsSuccess)
                        {
                            TipoEntidad = rsp.Result.Caratula.Entidad.TipoEntidad;
                            var rspArchivos = rsp.Result.Archivos;
                            setListArchivos(rspArchivos);
                            await grid.Reload();
                            StateHasChanged();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SessionId: Hubo un problema con la solicitud fetch: {ex.Message}");
            }
        }

        private void setListArchivos(List<ArchivoDTO> list)
        {
            if (list != null)
            {
                foreach (var x in list)
                {
                    var archivo = new ArchivoDTO
                    {
                        Id = x.Id,
                        SesionId = sesionId,
                        NombreArchivo = x.NombreArchivo,
                        Categoria = x.Categoria,
                        Hash = x.Hash,
                        Tama�o = x.Tama�o,
                        FechaCreacion = x.FechaCreacion
                    };
                    this.listArchivo.Add(archivo);
                }
            }
        }



        private bool checkData()
        {
            if (archivo.Categoria.IsNullOrEmpty())
            {
                msgErrorTipoArchivo = "";
            }
            else
            {
                msgErrorTipoArchivo = "El campo no puede estar vacio";
                return false;
            }

            msgErrorTipoArchivo = "";
            return true;
        }

        private async Task<ResponseDTO<BalanceDto>> DeleteArchivo(ArchivoDTO archivo)
        {
            var response = new ResponseDTO<BalanceDto>();
            try
            {
             

                response = await archivoService.deleteArchivo(archivo);
                
                if (response.IsSuccess)
                {
                    listArchivo.Remove(archivo);

                    await grid.Reload();
                    StateHasChanged();
                    
                   
                }

                else      {
                    response.Message = response.Message;

                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
           

            return response;
        }

        private async void OnProgress(UploadProgressArgs args)
        {
            var response = new ResponseDTO<BalanceDto>();
            try
            {
                if (checkData())
                {
                    if (args.Progress == 100)
                    {
                        var archivo = new ArchivoDTO();
                        foreach (var file in args.Files)
                        {
                            if (file.Size > 0)
                            {
                                //var binario = await ToByteArrayAsync(file.OpenReadStream(20 * 1024 * 1024)); // 20 MB
                                archivo.SesionId = sesionId;
                                archivo.Tama�o = file.Size;
                                archivo.ContentType = "pdf";
                                archivo.Categoria = categoria;
                                archivo.NombreArchivo = file.Name;
                                //archivo.Hash = Convert.ToHexString(SHA256.HashData(binario));
                                // Calcular el progreso de la carga


                                listArchivo.Add(archivo);
                                
                                response = await archivoService.UploadArchivo(listArchivo);
                               
                                if (response.IsSuccess)
                                {
                                    await grid.Reload();
                                    StateHasChanged();
                                }
                                else
                                {
                                    response.Message = $"Error uploading files";
                                }
                            }
                            else
                            {
                                msgError = $"El archivo {file.Name} est� vac�o. Por favor, seleccione un archivo v�lido.";
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = $"An error occurred while uploading files: {ex.Message}";
            }
        }



        private string conversionesDeArchivos(long file)
        {
            // Tama�o del archivo en bytes (esto podr�a provenir de tu archivo subido)
            long fileSizeInBytes = file;
            if (fileSizeInBytes < 1024)
            {
                return $"{Math.Round((double)fileSizeInBytes)} Bytes";
            }
            else if (fileSizeInBytes < 1024 * 1024)
            {
                double fileSizeInKB = (double)fileSizeInBytes / 1024;
                return $"{Math.Round(fileSizeInKB)} KB";
            }
            else if (fileSizeInBytes < 1024L * 1024 * 1024)
            {
                double fileSizeInMB = (double)fileSizeInBytes / (1024 * 1024);
                return $"{Math.Round(fileSizeInMB)} MB";
            }
            else if (fileSizeInBytes < 1024L * 1024 * 1024 * 1024)
            {
                double fileSizeInGB = (double)fileSizeInBytes / (1024 * 1024 * 1024);
                return $"{Math.Round(fileSizeInGB)} GB";
            }
            else
            {
                double fileSizeInTB = (double)fileSizeInBytes / (1024L * 1024 * 1024 * 1024);
                return $"{Math.Round(fileSizeInTB)} TB";
            }
        }
        private async Task<byte[]> ToByteArrayAsync(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}