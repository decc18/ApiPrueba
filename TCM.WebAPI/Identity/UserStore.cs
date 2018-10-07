using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TCM.Core.Entidades;
using TCM.Core.Util;
using TCM.WebAPI.Models;

namespace TCM.WebAPI.Identity
{
    public class UserStore : IUserStore<User>, IUserLoginStore<User>, IUserPasswordStore<User>, IUserSecurityStampStore<User>
    {
        private readonly string connectionString;

        public UserStore(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            this.connectionString = connectionString;
        }

        public UserStore()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public void Dispose()
        {

        }

        #region IUserStore
        public virtual Task CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return null;
            //return Task.Factory.StartNew(() => {
            //    user.UserId = Guid.NewGuid();
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        connection.Execute("insert into Users(UserId, UserName, PasswordHash, SecurityStamp) values(@userId, @userName, @passwordHash, @securityStamp)", user);
            //});
        }

        public virtual Task DeleteAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return null;
            //return Task.Factory.StartNew(() =>
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        connection.Execute("deete from Users where UserId = @userId", new { user.UserId });
            //});
        }

        public virtual Task<User> FindByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            Guid parsedUserId;
            if (!Guid.TryParse(userId, out parsedUserId))
                throw new ArgumentOutOfRangeException("userId", string.Format("'{0}' is not a valid GUID.", new { userId }));

            return Task.Factory.StartNew(() =>
            {
                var resp = LogicaNegocio.LogicaUsuarios.GetUsuarioByIdentificacion(userId);
                User model = ClsMapeadorEntidades.ConvertObject<User, DtoUsuario>(resp);
                return model;
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //    return connection.Query<User>("select * from Users where UserId = @userId", new { userId = parsedUserId }).SingleOrDefault();
            });
        }

        public DtoAplicacion FindClient(string idAplicacion)
        {
            if (string.IsNullOrWhiteSpace(idAplicacion))
                throw new ArgumentNullException("aplicacion");


            return LogicaNegocio.LogicaAplicacion.GetAplicacion(idAplicacion);
        }

        public Task<User> FindByNameAsync(string userName, string password, string idEmpresa)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(idEmpresa))
            {
                return Task.Factory.StartNew(() =>
                {
                    var resp = LogicaNegocio.LogicaUsuarios.GetUsuarioByUserName(userName, password, idEmpresa);
                    User model = new User { UserName = resp.Email, Id = resp.CodigoUsuario.ToString() };
                    return model;
                });
            }
            throw new ArgumentNullException("userName");
        }


        public virtual Task<User> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            return Task.Factory.StartNew(() =>
            {
                var resp = LogicaNegocio.LogicaUsuarios.GetUsuarioByUserName(userName);
                User model  = new User{ UserName = resp.Email, Id = resp.CodigoUsuario.ToString() }; 
                return model;
            });
        }

        public virtual Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return null;
            //return Task.Factory.StartNew(() =>
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        connection.Execute("update Users set UserName = @userName, PasswordHash = @passwordHash, SecurityStamp = @securityStamp where UserId = @userId", user);
            //});
        }
        #endregion

        #region IUserLoginStore
        public virtual Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return null;
            //return Task.Factory.StartNew(() =>
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        connection.Execute("insert into ExternalLogins(ExternalLoginId, UserId, LoginProvider, ProviderKey) values(@externalLoginId, @userId, @loginProvider, @providerKey)",
            //            new { externalLoginId = Guid.NewGuid(), userId = user.UserId, loginProvider = login.LoginProvider, providerKey = login.ProviderKey });
            //});
        }

        public virtual Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                var resp = LogicaNegocio.LogicaUsuarios.GetUsuarioByProveedor(login.LoginProvider, login.ProviderKey);
                User model = new User { UserName = resp.Email };
                return model;
            });
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return null;
            //return Task.Factory.StartNew(() =>
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        return (IList<UserLoginInfo>)connection.Query<UserLoginInfo>("select LoginProvider, ProviderKey from ExternalLogins where UserId = @userId", new { user.UserId }).ToList();
            //});
        }

        public virtual Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");
            return null;
            //return Task.Factory.StartNew(() =>
            //{
            //    using (SqlConnection connection = new SqlConnection(connectionString))
            //        connection.Execute("delete from ExternalLogins where UserId = @userId and LoginProvider = @loginProvider and ProviderKey = @providerKey",
            //            new { user.UserId, login.LoginProvider, login.ProviderKey });
            //});
        }
        #endregion

        #region IUserPasswordStore
        public virtual Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public virtual Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore
        public virtual Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        #endregion
    }
}