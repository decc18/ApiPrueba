using System;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.Core.LogicaNegocio;
using System.Collections.Generic;

namespace TCM.WebAPI.Controllers
{
    public class TipoVehiculoController : ApiController
    {

        [HttpPost]
        [Route("api/TipoVehiculos")]
        public async Task<IHttpActionResult> Get([FromBody] DtoTipoVehiculo item)
        {
            try
            {
                List<DtoTipoVehiculo> lista = new List<DtoTipoVehiculo>();


                await Task.Run(() =>
                {
                    lista = LogicaTipoVehiculo.ConsultarTipoVehiculo(item);

                });

                if (lista != null)
                {
                    
                    return Ok(lista);
                }                    
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
        [Route("api/PostTipoVehiculo")]
        public async Task<IHttpActionResult> Crear([FromBody]DtoTipoVehiculo item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaTipoVehiculo.CrearTipoVehiculo(item);
                    });

                    return Ok("Tipo de vehiculo creado");
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
        [Route("api/PutTipoVehiculo")]
        public async Task<IHttpActionResult> Edit([FromBody]DtoTipoVehiculo item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaTipoVehiculo.ActualizarTipoVehiculo(item);
                    });

                    return Ok("Tipo de vehiculo actualizado");
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
        [Route("api/DeleteTipoVehiculo")]
        public async Task<IHttpActionResult> Delete([FromBody]DtoTipoVehiculo item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaTipoVehiculo.EliminarTipoVehiculo(item);
                    });

                    return Ok("Tipo de vehiculo eliminado.");
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