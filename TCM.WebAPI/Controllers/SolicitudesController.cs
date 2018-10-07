using Common;
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
    public class SolicitudesController : ApiController
    {

        [HttpPost]
        [Route("api/Solicitudes")]
        public async Task<IHttpActionResult> Get([FromBody]DtoSolicitud filtros)
        {
            try
            {
                List<DtoSolicitud> lista = new List<DtoSolicitud>();


                await Task.Run(() =>
                {
                    lista = LogicaSolicitudes.GetSolicitudes(filtros);

                });

                if (lista != null)
                    return Ok(lista);
                else
                    return NotFound();

            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }
        }
        

        [HttpPost]
        [Route("api/PostSolicitud")]
        public async Task<IHttpActionResult> PostCatalogo([FromBody]DtoSolicitud item)
        {
            try
            {
                
                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaSolicitudes.CrearSolicitud(item);
                    });

                    return Ok("Solicitud creada");
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


        [HttpPut]
        [Route("api/PutSolicitud")]
        public async Task<IHttpActionResult> PutCatalogo([FromBody]DtoSolicitud item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaSolicitudes.ActualizarSolicitud(item);
                    });

                    return Ok("Solicitud actualizada");
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