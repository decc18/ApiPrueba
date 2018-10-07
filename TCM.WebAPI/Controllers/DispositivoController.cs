using System;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.Core.LogicaNegocio;
using System.Collections.Generic;
using System.Linq;

namespace TCM.WebAPI.Controllers
{
    [Authorize]
    public class DispositivoController : ApiController
    {
        [HttpPost]
        [Route("api/Dispositivos")]
        public async Task<IHttpActionResult> Get(DtoDispositivo item)
        {
            try
            {
                List<DtoDispositivo> lista = new List<DtoDispositivo>();


                await Task.Run(() =>
                {
                    lista = LogicaDispositivo.ConsultarDispositivo(item);

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
        [Route("api/PostDispositivo")]
        public async Task<IHttpActionResult> Crear([FromBody]DtoDispositivo item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        var dispositivo = LogicaDispositivo.ConsultarDispositivo(new DtoDispositivo { NumeroSerie = item.NumeroSerie });
                        if (dispositivo.Count > 0)
                        {
                            item.Id = dispositivo.FirstOrDefault().Id;
                            LogicaDispositivo.ActualizarDispositivo(item);
                        }
                        else
                            LogicaDispositivo.CrearDispositivo(item);

                    });

                    return Ok("Dispositivo creado");
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
        [Route("api/PutDispositivo")]
        public async Task<IHttpActionResult> Edit(DtoDispositivo item)
        {
            try
            {
                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaDispositivo.ActualizarDispositivo(item);
                    });

                    return Ok("Dispositivo actualizado");
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
        [Route("api/DeleteDispositivo")]
        public async Task<IHttpActionResult> Delete(DtoDispositivo item)
        {
            try
            {
                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        item.Estado = EnumEstados.Eliminado;
                        LogicaDispositivo.EliminarDispositivo(item);
                    });

                    return Ok("Dispositivo eliminado.");
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