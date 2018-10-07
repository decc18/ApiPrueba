using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.LogicaNegocio;
using TCM.Core.Util;
using TCM.LogicaNegocio;


namespace TCM.WebAPI.Controllers
{
    public class FileTransferController : ApiController
    {
        
        [HttpPost]
        [Route("api/PostFileTransfer")]
        public async Task<IHttpActionResult> PostCatalogo([FromBody]DtoFileTransfer msg)
        {
            try
            {
                string resumen = string.Empty;

                if (msg != null)
                {
                    await Task.Run(() =>
                    {
                        switch (msg.Tipo)
                        {
                            case EnumProcesoMasivo.DetalleCatalogo:
                                resumen = LogicaCatalogos.ActualizarCatalogo(msg);
                                break;
                            case EnumProcesoMasivo.LoadImage:
                                
                                break;
                            default:
                                ClsVisorEventos.LogAlert(msg.Tipo, "No implementado");                                
                                break;
                        }                        
                    });

                    return Ok(resumen);
                }
                else
                    return BadRequest("Incorrect call");
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

        }


    }
}