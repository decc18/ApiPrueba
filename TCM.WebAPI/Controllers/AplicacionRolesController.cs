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
    public class AplicacionRolesController : ApiController
    {
        private TransRabbitDbContext db = new TransRabbitDbContext();

        // GET: api/AplicacionRoles
        public IQueryable<AplicacionRol> GetRoles()
        {
            return db.IdentityRoles;
        }

        // GET: api/AplicacionRoles/5
        [ResponseType(typeof(AplicacionRol))]
        public IHttpActionResult GetAplicacionRol(string id)
        {
            AplicacionRol aplicacionRol = db.IdentityRoles.Find(id);
            if (aplicacionRol == null)
            {
                return NotFound();
            }

            return Ok(aplicacionRol);
        }

        // PUT: api/AplicacionRoles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAplicacionRol(string id, AplicacionRol aplicacionRol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aplicacionRol.Id)
            {
                return BadRequest();
            }

            db.Entry(aplicacionRol).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AplicacionRolExists(id))
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

        // POST: api/AplicacionRoles
        [ResponseType(typeof(AplicacionRol))]
        public IHttpActionResult PostAplicacionRol(AplicacionRol aplicacionRol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Roles.Add(aplicacionRol);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AplicacionRolExists(aplicacionRol.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aplicacionRol.Id }, aplicacionRol);
        }

        // DELETE: api/AplicacionRoles/5
        [ResponseType(typeof(AplicacionRol))]
        public IHttpActionResult DeleteAplicacionRol(string id)
        {
            AplicacionRol aplicacionRol = db.IdentityRoles.Find(id);
            if (aplicacionRol == null)
            {
                return NotFound();
            }

            db.Roles.Remove(aplicacionRol);
            db.SaveChanges();

            return Ok(aplicacionRol);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AplicacionRolExists(string id)
        {
            return db.Roles.Count(e => e.Id == id) > 0;
        }
    }
}