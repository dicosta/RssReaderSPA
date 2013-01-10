using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssReader.Model;
using RssReader.Services.Contracts;

namespace RssReader.Web.Infrastructure.Auth
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        public User GetCurrentUser()
        {
            throw new NotImplementedException();
        }
    }
}