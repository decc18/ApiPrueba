using System;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.Core.LogicaNegocio;
using System.Collections.Generic;

namespace TCM.WebAPI.Controllers
{
    public class VehiculosController : ApiController
    {
        [HttpPost]
        [Route("api/Vehiculos")]
        public async Task<IHttpActionResult> Get(DtoVehiculo item)
        {
            try
            {
                List<DtoVehiculo> lista = new List<DtoVehiculo>();


                await Task.Run(() =>
                {
                    lista = LogicaVehiculos.ConsultarVehiculo(item);

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
        [Route("api/PostVehiculo")]
        public async Task<IHttpActionResult> Crear([FromBody]DtoVehiculo item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaVehiculos.CrearVehiculo(item);
                    });

                    return Ok("Vehiculo creado");
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
        [Route("api/PutVehiculo")]
        public async Task<IHttpActionResult> Edit(DtoVehiculo item)
        {
            try
            {
                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaVehiculos.ActualizarVehiculo(item);
                    });

                    return Ok("Vehiculo actualizado");
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
        [Route("api/DeleteVehiculo")]
        public async Task<IHttpActionResult> Delete(DtoVehiculo item)
        {
            try
            {
                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        item.Estado = EnumEstados.Eliminado;
                        LogicaVehiculos.ActualizarVehiculo(item);
                    });

                    return Ok("Vehiculo eliminado.");
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