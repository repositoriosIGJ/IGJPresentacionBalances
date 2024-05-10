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


        RadzenUpload upload = new RadzenUpload();
        UploadProgressArgs args = new UploadProgressArgs();

        int progress;
        bool showProgress;
        bool showComplete;
        string completionMessage;
        bool cancelUpload = false;


        [Parameter]
        public string? TipoEntidad { get; set; }

        [Parameter]
        public string? balid { get; set; }

        [Parameter]
        public string sesionId { get; set; }

 
        private string msgError = "";
        private string msgErrorTipoArchivo = "";

        private ArchivoDTO archivo = new ArchivoDTO();
        private List<ArchivoDTO> listArchivo = new List<ArchivoDTO>();
       
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
                    sesionId = sesionRespuesta.Result;
                    sessionStorage.SetItemAsync("SessionId", sesionId);
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

                    archivo.Id = x.Id;
                    archivo.SesionId = sesionId;
                    archivo.NombreArchivo = x.NombreArchivo;
                    archivo.Categoria = x.Categoria;
                    archivo.Hash = x.Hash;
                    archivo.Tama�o = x.Tama�o;
                    archivo.FechaCreacion = x.FechaCreacion;

                    listArchivo.Add(archivo);
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

        private async Task<ResponseDTO<BalanceDto>> deleteArchivo(ArchivoDTO archivo)
        {
            var response = new ResponseDTO<BalanceDto>();
            try
            {
                response = await archivoService.deleteArchivo(archivo);
                if (response.IsSuccess)
                {
                    listArchivo.Remove(archivo);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        private async Task<byte[]> ToByteArrayAsync(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }






        async Task OnProgress(UploadProgressArgs args)
        {
            this.args = args;
            
            try
            {
                if (this.args.Progress == 100)
                {
                    foreach (var file in this.args.Files)
                    {
                        if (file.Size > 0)
                        {
                            var binario = await ToByteArrayAsync(file.OpenReadStream(20 * 1024 * 1024)); // 20 MB
                            var archivo = new ArchivoDTO(); // Crear una nueva instancia para cada archivo
                            archivo.SesionId = sesionId;
                            archivo.Tama�o = file.Size;
                            archivo.ContentType = file.ContentType;
                            archivo.NombreArchivo = file.Name;
                            archivo.Hash = Convert.ToHexString(SHA256.HashData(binario));

                            listArchivo.Add(archivo);
                        }
                        else
                        {
                            msgError = $"El archivo {file.Name} est� vac�o. Por favor, seleccione un archivo v�lido.";
                        }
                    }
                    var response = await archivoService.UploadArchivo(listArchivo);
                    if (response.IsSuccess)
                    {
                        StateHasChanged();
                    }
                    else
                    {
                        // Mostrar mensaje de error en caso de fallo en la carga
                        response.Message = $"Error uploading files";
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepci�n que ocurra durante la carga
                msgError = $"An error occurred while uploading files: {ex.Message}";
            }
        }

        // M�todo para iniciar la carga de archivos
        public async Task UploadFiles()
        {
            try
            {
                await upload.Upload(); // Esperar la carga de archivos

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir archivos: {ex.Message}");
            }
        }


    }
}