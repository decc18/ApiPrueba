using System;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.Core.LogicaNegocio;
using System.Collections.Generic;

namespace TCM.WebAPI.Controllers
{
    public class DestinoController : ApiController
    {
        [HttpPost]
        [Route("api/Destinos")]
        public async Task<IHttpActionResult> Get([FromBody] DtoDestino item)
        {
            try
            {
                List<DtoDestino> lista = new List<DtoDestino>();


                await Task.Run(() =>
                {
                    lista = LogicaDestino.ConsultarDestinos(item);

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
        [Route("api/PostDestino")]
        public async Task<IHttpActionResult> Create([FromBody]DtoDestino item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaDestino.CrearDestino(item);
                    });

                    return Ok();
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
        [Route("api/PutDestino")]
        public async Task<IHttpActionResult> Edit([FromBody]DtoDestino item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaDestino.ActualizarDestino(item);
                    });

                    return Ok();
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
        [Route("api/DeleteDestino")]
        public async Task<IHttpActionResult> Delete([FromBody]DtoDestino item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaDestino.EliminarDestino(item);
                    });

                    return Ok();
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