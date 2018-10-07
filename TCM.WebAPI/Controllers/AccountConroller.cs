using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using TCM.WebAPI.Models;
using TCM.WebAPI.Results;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.WebAPI.Identity;
using Newtonsoft.Json.Linq;

namespace TCM.WebAPI.Controllers
{

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        public AccountController()
            : this(new UserManager<User>(new UserStore()))
        {
        }

        public AccountController(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }


        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }


        //POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objUsuario = LogicaNegocio.LogicaUsuarios.GetUsuarioByEmail(model.Email);

            bool hasRegistered = !string.IsNullOrEmpty(objUsuario.CodigoUsuario.ToString());

            if (hasRegistered)
            {
                return BadRequest("El usuario ya está registrado");
            }

            LogicaNegocio.LogicaUsuarios.CrearUsuario(new DtoUsuario
            {
                Email = model.Email,
                Estado = EnumEstados.Activo,
                TipoId = model.TipoIdentificacion,
                Password = model.Password,
                Identificacion = model.Identificacion,
                NumeroContacto = model.Phone,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos
            });


            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var objUsuario = LogicaNegocio.LogicaUsuarios.GetUsuarioByEmail(externalLogin.Email);
            //Registro autenticación externa
            if (!string.IsNullOrEmpty(objUsuario.CodigoUsuario.ToString()))
            {
                using (UserStore _repo = new UserStore())
                {
                    var userExternal = await _repo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));
                    if (string.IsNullOrEmpty(userExternal.UserName))
                    {
                        LogicaNegocio.LogicaUsuarios.CrearUsuarioExterno(new DtoAutenticacionExterna
                        {
                            Email = externalLogin.Email,
                            LlaveProveedor = externalLogin.ProviderKey,
                            Proveedor = externalLogin.LoginProvider
                        });
                    }
                }
            }

            bool hasRegistered = !string.IsNullOrEmpty(objUsuario.CodigoUsuario.ToString());

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&urlPicture{5}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.Email,
                                            externalLogin.UrlPicture);

            return Redirect(redirectUri);

        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Proveedor, model.AccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Proveedor no válido o token de acceso externo");
            }

            User user = null;
            using (UserStore _repo = new UserStore())
            {
                user = await _repo.FindAsync(new UserLoginInfo(model.Proveedor, verifiedAccessToken.user_id)); 
            }

            bool hasRegistered = string.IsNullOrEmpty(user.UserName);

            if (!hasRegistered)
            {
                return BadRequest("El usuario externo ya está registrado");
            }
            
            LogicaNegocio.LogicaUsuarios.CrearUsuario(new DtoUsuario
            {
                Email = model.Email,
                Estado = EnumEstados.Activo,
                TipoId = model.TipoIdentificacion,
                Password = model.Password,
                Identificacion = model.Identificacion,
                NumeroContacto = model.Phone,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos
            });

            LogicaNegocio.LogicaUsuarios.CrearUsuarioExterno(new DtoAutenticacionExterna
            {
                Email = model.Email,
                LlaveProveedor = model.LlaveProveedor,
                Proveedor = model.Proveedor
            });

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.Email);

            return Ok(accessTokenResponse);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            User user = null;
            using (UserStore _repo = new UserStore())
            {
                user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));
            }

            bool hasRegistered = !string.IsNullOrEmpty(user.UserName);

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

            return Ok(accessTokenResponse);

        }


        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            DtoAplicacion aplicacion = null;
            using (UserStore _repo = new UserStore())
            {
                aplicacion = _repo.FindClient(clientId);
            }

            if (aplicacion == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            //if (!string.Equals(aplicacion.UrlPermitida, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            //{
            //    return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            //}

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                var appToken = "815854768614603|cE76F0l7RyIkLyPbHOqDLxV3RVk";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else if (provider == "Google")
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }

            }

            return parsedToken;
        }

        private JObject GenerateLocalAccessTokenResponse(string email)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, email));
            identity.AddClaim(new Claim(ClaimTypes.Role, ERolNiveles.PermisosUsuario.ToString()));
            identity.AddClaim(new Claim("sub", email));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", email),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string Email { get; set; }
            public string ExternalAccessToken { get; set; }
            public string UrlPicture { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                
               
                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    Email = identity.FindFirstValue(ClaimTypes.Email),
                    ExternalAccessToken = string.IsNullOrEmpty(identity.FindFirstValue("urn:google:accesstoken")) ? identity.FindFirstValue("ExternalAccessToken") : identity.FindFirstValue("urn:google:accesstoken"),
                    UrlPicture = identity.FindFirstValue("PictureUrl")
                };
            }
        }

        #endregion
    }
}