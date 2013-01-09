using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using RssReader.Services.Contracts;
using RssReader.Web.Models;

namespace RssReader.Web.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService userService;
        public readonly ICurrentUserProvider currentUserProvider;
        public readonly IFeedService feedService;

        public UserController(IUserService userService, ICurrentUserProvider currentUserProvider,
            IFeedService feedService)
        {
            this.userService = userService;
            this.currentUserProvider = currentUserProvider;
            this.feedService = feedService;
        }

        public UserViewModel GetUser()
        {
            var user = currentUserProvider.GetCurrentUser();

            var vm = new UserViewModel(user);
            foreach (Guid feedId in user.Feeds)
            {
                vm.Feeds.Add(new FeedViewModel(feedService.GetById(feedId)));
            }

            return vm; 
        }

        public void AddCategory(string categoryName)
        {
            userService.AddCategory(categoryName);
        }

        public void RemoveCategory(string categoryName)
        {
            userService.RemoveCategory(categoryName);
        }

        public void SuscribeFeed(string url)
        {
            feedService.Suscribe(url);
        }

        public void UnsuscribeFeed(Guid id)
        {
            feedService.Unsuscribe(id);
        }
    }
}
