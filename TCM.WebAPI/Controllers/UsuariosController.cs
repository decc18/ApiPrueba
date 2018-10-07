using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.LogicaNegocio;
using TCM.WebAPI.Clases;

namespace TCM.WebAPI.Controllers
{
    public class UsuariosController : ApiController
    {

        [HttpPost]
        [Route("api/Usuarios")]
        public List<DtoUsuario> GetUsuarios(DtoUsuario filtros)
        {
            List<DtoUsuario> usuarios = LogicaUsuarios.GetUsuarios(filtros);
            return usuarios;
        }

        [Route("api/UsuarioById")]
        [ResponseType(typeof(DtoUsuario))]
        public IHttpActionResult GetUsuario(string Id)
        {           
            DtoUsuario usuario = LogicaUsuarios.GetUsuarioByIdentificacion(Id);
            
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [Route("api/PutUsuario")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUsuario(DtoUsuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                LogicaUsuarios.ActualizarUsuario(usuario);
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/PostUsuario")]
        [ResponseType(typeof(DtoUsuario))]
        public IHttpActionResult PostAppUsuario(DtoUsuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                LogicaUsuarios.CrearUsuario(usuario);
                return Ok();
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }
        }


        [HttpPost]
        [Route("api/DeleteUsuario")]
        [ResponseType(typeof(DtoUsuario))]
        public IHttpActionResult DeleteAppUsuario(DtoUsuario item)
        {
            LogicaUsuarios.EliminarUsuario(item);
            return Ok(item);
        }


        [HttpPost]
        [Authorize]
        [Route("api/Usuarios/ActualizacionUsuario")]
        public IHttpActionResult ActualizacionUsuario(JObject form)
        {
            var identification = string.Empty;
            var email = string.Empty;
            var phone = string.Empty;
            dynamic jsonObject = form;

            try
            {
                phone = jsonObject.Phone.Value;
                email = jsonObject.Email.Value;
                identification = jsonObject.Identification.Value;
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

            DtoUsuario usuario = LogicaUsuarios.GetUsuarioByEmail(email);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.Email = email;
            usuario.NumeroContacto = phone;

            try
            {
                LogicaUsuarios.ActualizarUsuario(usuario);
            }
            catch (Exception ex)
            {
                ClsVisorEventos.LogEvent(ex);
                return BadRequest($"Incorrect call:{ex.Message}");
            }

            return Ok(true);
        }


        [HttpPost]
        [Route("api/Usuarios/RecuperarPassword")]
        public async Task<IHttpActionResult> RecuperarPassword(JObject form)
        {
            try
            {
                var identificacion = string.Empty;
                var email = string.Empty;
                dynamic jsonObject = form;

                try
                {
                    identificacion = jsonObject.Identificacion.Value;
                    email = jsonObject.Email.Value;
                }
                catch (Exception ex)
                {
                    ClsVisorEventos.LogEvent(ex);
                    return BadRequest($"Incorrect call:{ex.Message}");
                }

                DtoUsuario usuario = LogicaUsuarios.GetUsuarioByEmail(email);
                if (usuario == null)
                {
                    return NotFound();
                }
                if (usuario.Email.Trim().ToUpper() != email.Trim().ToUpper())
                {
                    return BadRequest("Su password no pude ser enviada!");
                }
                if (usuario.Identificacion.Trim() != identificacion.Trim())
                {
                    return BadRequest("Su password no pude ser enviada!");
                }

                var subject = WebConfigurationManager.AppSettings["Subject"];
                var body = Util.PopulateBody(usuario.Nombres, usuario.Password);

                await Util.EnviarMail(usuario.Email, subject, body);
                return Ok(true);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        


    }
}