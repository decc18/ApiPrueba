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
    public class HerramientasController : ApiController
    {
        

        [HttpGet]
        [Route("api/GetEstado")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                string resp = string.Empty;

                await Task.Run(() =>
                {
                     resp = LogicaGeneral.fnTestConexionBDD();

                });

                return Ok(resp);
                
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }
        }
    }
}