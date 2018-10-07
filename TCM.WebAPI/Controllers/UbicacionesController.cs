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
    public class UbicacionesController : ApiController
    {
        [HttpPost]
        [Route("api/Ubicaciones")]
        public async Task<IHttpActionResult> GetUbicaciones(DtoUbicacion item)
        {
            try
            {
                List<DtoUbicacion> consulta = new List<DtoUbicacion>();


                await Task.Run(() =>
                {
                    consulta = LogicaUbicaciones.GetUbicaciones(item);

                });

                if (consulta != null)
                    return Ok(consulta);
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
        [Route("api/PostUbicacion")]
        public async Task<IHttpActionResult> PostUbicacion(DtoUbicacion Ubicacion)
        {
            try
            {
                //if (!ModelState.IsValid)                
                //    return BadRequest(ModelState);


                if (Ubicacion != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaUbicaciones.CrearUbicacion(Ubicacion);
                    });

                    return Ok(Ubicacion);
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
        [Route("api/PutUbicacion")]
        public async Task<IHttpActionResult> PutUbicacion(DtoUbicacion Ubicacion)
        {
            try
            {

                if (Ubicacion != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaUbicaciones.ActualizarUbicacion(Ubicacion);
                    });

                    return Ok("Actualizado con éxito");
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

        [HttpPost]
        [Route("api/DeleteUbicacion")]
        public async Task<IHttpActionResult> DeleteUbicacion(DtoUbicacion item)
        {
            try
            {
                await Task.Run(() =>
                {
                    LogicaUbicaciones.EliminarUbicacion(item);
                });

                return Ok();

            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

        }


    }
}