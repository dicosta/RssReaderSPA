using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssReader.Model;
using RssReader.Services.Contracts;
using WebMatrix.WebData;


namespace RssReader.Web.Infrastructure.Auth
{
    public class CurrentUserIdProvider : ICurrentUserIdProvider
    {
        private readonly IUserService userService;

        public Guid GetCurrentUserId()
        {
            var userId = WebSecurity.CurrentUserId;
            //return null;
            throw new NotImplementedException();
        }
    }
}