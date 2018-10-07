using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TCM.Core.Util;

namespace TCM.WebAPI.Models
{

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class RegisterExternalBindingModel
    {


        [Required]
        public string Email { get; set; }

        [Required]
        public string Proveedor { get; set; }

        [Required]
        public string LlaveProveedor { get; set; }

        [Required]
        public string Identificacion { get; set; }

        [Required]
        public EnumTipoIdentificacion TipoIdentificacion { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public string AccessToken { get; set; }

    }

    public class RegisterBindingModel
    {

        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Identificacion { get; set; }

        [Required]
        public EnumTipoIdentificacion TipoIdentificacion { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Apellidos { get; set; }
    }

    public class ParsedExternalAccessToken
    {
        public string user_id { get; set; }
        public string app_id { get; set; }
    }
}