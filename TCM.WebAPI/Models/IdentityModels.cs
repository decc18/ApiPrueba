using System;
using Microsoft.AspNet.Identity;

namespace TCM.WebAPI.Models
{
    public class User : IUser
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        public string Id { get; set; }
    }
}