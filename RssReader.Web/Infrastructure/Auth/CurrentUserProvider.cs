using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RssReader.Model;
using RssReader.Services.Contracts;
using WebMatrix.WebData;

namespace RssReader.Web.Infrastructure.Auth
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        //private readonly IUserService userService;

        public CurrentUserProvider(//IUserService userService)
            )
        {
            //this.userService = userService;
        }

        public User GetCurrentUser()
        {
            throw new Exception("no poner userservice aca");
            //return userService.GetByUserName(WebSecurity.CurrentUserName);
        }
    }
}