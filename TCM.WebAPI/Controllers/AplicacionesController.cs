using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Data;
using Models;

namespace TCM.WebAPI.Controllers
{
    public class AplicacionesController : ApiController
    {
        private TransRabbitDbContext db = new TransRabbitDbContext();

        // GET: api/Aplicaciones
        public IQueryable<Aplicacion> GetAplicacions()
        {
            return db.Aplicaciones;
        }

        // GET: api/Aplicaciones/5
        [ResponseType(typeof(Aplicacion))]
        public IHttpActionResult GetAplicacion(string id)
        {
            Aplicacion aplicacion = db.Aplicaciones.Find(id);
            if (aplicacion == null)
            {
                return NotFound();
            }

            return Ok(aplicacion);
        }

        // PUT: api/Aplicaciones/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAplicacion(string id, Aplicacion aplicacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aplicacion.Id)
            {
                return BadRequest();
            }

            db.Entry(aplicacion).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AplicacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Aplicaciones
        [ResponseType(typeof(Aplicacion))]
        public IHttpActionResult PostAplicacion(Aplicacion aplicacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Aplicaciones.Add(aplicacion);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AplicacionExists(aplicacion.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aplicacion.Id }, aplicacion);
        }

        // DELETE: api/Aplicaciones/5
        [ResponseType(typeof(Aplicacion))]
        public IHttpActionResult DeleteAplicacion(string id)
        {
            Aplicacion aplicacion = db.Aplicaciones.Find(id);
            if (aplicacion == null)
            {
                return NotFound();
            }

            db.Aplicaciones.Remove(aplicacion);
            db.SaveChanges();

            return Ok(aplicacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AplicacionExists(string id)
        {
            return db.Aplicaciones.Count(e => e.Id == id) > 0;
        }
    }
}