using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssReader.Model;
using RssReader.Services.Contracts;
using WebMatrix.WebData;


namespace RssReader.Web.Infrastructure.Auth
{
    public class CurrentUserNameProvider : ICurrentUserNameProvider
    {        
        public string GetCurrentUserName()
        {
            return WebSecurity.CurrentUserName;
        }
    }
}