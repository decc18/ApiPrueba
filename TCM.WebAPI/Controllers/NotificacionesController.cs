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
using TCM.WebAPI.Clases;
using TCM.WebAPI.Hubs;

namespace TCM.WebAPI.Controllers
{
    public class NotificacionesController : ApiController
    {

        [HttpPost]
        [Route("api/Notificaciones")]
        public async Task<IHttpActionResult> Get(DtoNotificacion filtros)
        {
            try
            {
                
                List<DtoNotificacion> lista = new List<DtoNotificacion>();

                await Task.Run(() =>
                {
                    lista = LogicaNotificaciones.GetNotificaciones(filtros);

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

        [Route("api/PushNotificacion")]
        public void Post(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                InternalHub hub = new InternalHub();
                hub.EnviarNotificaciones(email);
            }
        }

        private string CargarNotificacionesUsuario(DtoUsuario usuario)
        {
            int total = 0;
            var lista = LogicaNotificaciones.GetNotificaciones(new DtoNotificacion { IdDestino = usuario.CodigoUsuario });
            total = lista.Count;
            return total.ToString();
        }

        [HttpPost]
        [Route("api/PostNotificacion")]
        public async Task<IHttpActionResult> Post([FromBody]DtoNotificacion item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaNotificaciones.CrearNotificacion(item);
                    });
                    //Envia notificaciòn al usuario destino
                    InternalHub hub = new InternalHub();
                    DtoUsuario usuario = LogicaUsuarios.GetUsuarioByCodigoUsuario((int)item.IdDestino);
                    hub.EnviarNotificaciones(usuario.Email);
                    return Ok("Notificacion creada");
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
        [Route("api/PutNotificacion")]
        public async Task<IHttpActionResult> Put([FromBody]DtoNotificacion item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaNotificaciones.ActualizarNotificacion(item);
                    });

                    return Ok("Notificacion actualizada");
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
        [Route("api/DeleteNotificacion")]
        public async Task<IHttpActionResult> Delete([FromBody]DtoNotificacion item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        LogicaNotificaciones.EliminarNotificacion(item);
                    });
                    
                    return Ok("Notificación eliminada.");
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
        [Route("api/PostNotificacionPush")]
        public async Task<IHttpActionResult> Post([FromBody]DtoNotificacionPush item)
        {
            try
            {

                if (item != null)
                {
                    await Task.Run(() =>
                    {
                        var listDispositivos = LogicaDispositivo.ConsultarDispositivo(new DtoDispositivo { Id = Convert.ToInt32(item.IdDispositivo) });
                        Util.EnviarNotificacionPushFCM(listDispositivos, item.Titulo, item.Mensaje);
                        LogicaNotificaciones.CrearNotificacionPush(item);
                    });
                    return Ok("Notificacion creada");
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