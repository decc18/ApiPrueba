using Common;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using TCM.Core.Entidades;
using TCM.Core.LogicaNegocio;
using TCM.Core.Util;
using TCM.LogicaNegocio;
using TCM.WebAPI.Models;

namespace TCM.WebAPI.Hubs
{
    [HubName("InternalHub")]
    //[Authorize]
    public class InternalHub : Hub
    {

        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
           new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);

        public void SendCpuInfo(string machineName, double processor, int memUsage, int totalMemory)
        {
            this.Clients.All.cpuInfoMessage(machineName, processor, memUsage, totalMemory);
        }



        public void ObtenerNotificaciones()
        {
            try
            {
                string email = Context.User.Identity.Name;

                //Get TotalNotification

                //Send To
                if (!string.IsNullOrEmpty(email))
                {
                    DtoUsuario usuario = LogicaUsuarios.GetUsuarioByIdentificacion(email);

                    if (!string.IsNullOrEmpty(usuario.Identificacion))
                    {
                        var cid = email;
                        var context = GlobalHost.ConnectionManager.GetHubContext<InternalHub>();
                       
                    }

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        //Specific User Call
        public void EnviarNotificaciones(string email)
        {
            try
            {
                UserHubModels user;
                Users.TryGetValue(email, out user);

                if (user != null)
                {
                    var cid = user.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<InternalHub>();
                    context.Clients.Client(cid).notificacionesPendientes(email);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }


        public override Task OnConnected()
        {
            string userName = Context.QueryString["username"];
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubModels
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.QueryString["username"];
            string connectionId = Context.ConnectionId;

            UserHubModels user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

       

    }
}