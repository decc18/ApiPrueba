using System;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.Core.LogicaNegocio;
using System.Collections.Generic;
using System.IO;

namespace TCM.WebAPI.Controllers
{
    public class SitioTuristicoController : ApiController
    {
        [HttpPost]
        [Route("api/SitiosTuristicos")]
        public async Task<IHttpActionResult> Get([FromBody] DtoSitioTuristico item)
        {
            try
            {
                List<DtoSitioTuristico> lista = new List<DtoSitioTuristico>();


                await Task.Run(() =>
                {
                    lista = LogicaSitioTuristico.ConsultarSitiosTuristicos(item);

                });

                if (lista != null)
                {

                    foreach (DtoSitioTuristico s in lista)
                    {
                        FileInfo f = new FileInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\Recursos\\SitioTuristico{s.CodigoSitio}.jpg");

                        if (!f.Exists)
                        {
                            using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(s.Logo)))
                                image.Save(f.FullName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }

                        s.Logo = null;
                    }

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
        [Route("api/PostSitioTuristico")]
        public async Task<IHttpActionResult> Create([FromBody]DtoSitioTuristico item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaSitioTuristico.CrearSitioTuristico(item);
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
        [Route("api/PutSitioTuristico")]
        public async Task<IHttpActionResult> Edit([FromBody]DtoSitioTuristico item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaSitioTuristico.ActualizarSitioTuristico(item);
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
        [Route("api/DeleteSitioTuristico")]
        public async Task<IHttpActionResult> Delete([FromBody]DtoSitioTuristico item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaSitioTuristico.EliminarSitioTuristico(item);
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