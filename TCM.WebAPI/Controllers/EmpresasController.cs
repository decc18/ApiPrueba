
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.LogicaNegocio;

namespace TCM.WebAPI.Controllers
{
    public class EmpresasController : ApiController
    {

        [HttpPost]
        [Route("api/Empresas")]
        public async Task<IHttpActionResult> Get(DtoEmpresa filtros)
        {
            try
            {
                List<DtoEmpresa> empresas = new List<DtoEmpresa>();


                await Task.Run(() =>
                {
                    empresas = LogicaEmpresas.GetEmpresas(filtros); 

                });

                if (empresas != null)
                    return Ok(empresas);
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
        [Route("api/PostEmpresa")]
        public async Task<IHttpActionResult> PostEmpresa([FromBody]DtoEmpresa empresa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (empresa != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaEmpresas.CrearEmpresa(empresa);

                    });

                    return Ok(empresa);
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
        [Route("api/PutEmpresa")]
        public async Task<IHttpActionResult> Putempresa([FromBody]DtoEmpresa empresa)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (empresa != null)
                {
                    await Task.Run(() =>
                    {

                        LogicaEmpresas.ActualizarEmpresa(empresa);

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


    }
}