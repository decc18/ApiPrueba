using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.LogicaNegocio;

namespace TCM.WebAPI.Controllers
{
    //[AuthorizeEnum(ERolNiveles.PermisosUsuario)]
    //[Authorize]
    public class CatalogosController : ApiController
    {
        [HttpGet]
        [Route("api/ExportaCatalogo")]
        public async Task<IHttpActionResult> ExportaCatalogo(string NombreCatalogo)
        {
            MemoryStream stream = LogicaCatalogos.GetExelDetallesCatalogo(NombreCatalogo);

            HttpResponseMessage result = CrearAttachment(stream, $"{NombreCatalogo}.{EnumMIME.xlsx}");
            
            var response = ResponseMessage(result);

            return response;
        }


        HttpResponseMessage CrearAttachment(MemoryStream stream, string NombreArchivo)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.GetBuffer())
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = NombreArchivo
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }


        [HttpPost]
        [Route("api/Catalogos")]
        public async Task<IHttpActionResult> GetCatalogos(DtoCatalogo item)
        {
            try
            {
                List<DtoCatalogo> catalogos = new List<DtoCatalogo>();


                await Task.Run(() =>
                {
                    catalogos = LogicaCatalogos.GetCatalogos(item);

                });

                if (catalogos != null)
                    return Ok(catalogos);
                else
                    return NotFound();

                //return BadRequest("Incorrect call");
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }
        }
        
        
        [HttpPost]
        [Route("api/PostCatalogo")]
        public async Task<IHttpActionResult> PostCatalogo(DtoCatalogo catalogo)
        {
            try
            {
                //if (!ModelState.IsValid)                
                //    return BadRequest(ModelState);
                

                if (catalogo != null )
                {
                    await Task.Run(() =>
                    {

                        LogicaCatalogos.CrearCatalogo(catalogo);

                    });
                    
                    return Ok(catalogo);                    
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
        [Route("api/PutCatalogo")]
        public async Task<IHttpActionResult> PutCatalogo(DtoCatalogo catalogo)
        {
            try
            {
                //if (!ModelState.IsValid)                
                //    return BadRequest(ModelState);
                

                if (catalogo != null)
                {
                    await Task.Run(() =>
                    {

                        LogicaCatalogos.ActualizarCatalogo(catalogo);

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
        [Route("api/DeleteCatalogo")]
        public async Task<IHttpActionResult> DeleteCatalogo(DtoCatalogo item)
        {
            try
            {
                
                await Task.Run(() =>
                {
                    LogicaCatalogos.EliminarCatalogo(item.NombreCatalogo);
                });

                return Ok(item.NombreCatalogo);
                
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

        }



        #region Detalles

        [HttpPost]
        [Route("api/PostDetalle")]
        public async Task<IHttpActionResult> PostDetalle(DtoDetalleCatalogo detalle)
        {
            try
            {
                //if (!ModelState.IsValid)                
                //    return BadRequest(ModelState);


                if (detalle != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaCatalogos.CrearDetalleCatalogo(detalle);
                    });

                    return Ok(detalle);
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
        [Route("api/PutDetalle")]
        public async Task<IHttpActionResult> PutDeatelle([FromBody]DtoDetalleCatalogo detalle)
        {
            try
            {
                //if (!ModelState.IsValid)                
                //    return BadRequest(ModelState);
                

                if (detalle != null)
                {
                    await Task.Run(() =>
                    {

                        LogicaCatalogos.ActualizarDetalleCatalogo(detalle);

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
        [Route("api/DeleteDetalle")]
        public async Task<IHttpActionResult> DeleteDeatelle(DtoDetalleCatalogo item)
        {
            try
            {
                await Task.Run(() =>
                {
                    LogicaCatalogos.EliminarDetalleCatalogo(item);
                });

                return Ok("Actualizado con éxito");
                
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

        }





        #endregion Detalles

    }
}