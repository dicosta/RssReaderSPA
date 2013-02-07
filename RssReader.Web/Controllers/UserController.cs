using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using RssReader.Services.Contracts;
using RssReader.Web.Models;
using WebMatrix.WebData;

namespace RssReader.Web.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService userService;
        public readonly IFeedService feedService;

        public UserController(IUserService userService,
            IFeedService feedService)
        {
            this.userService = userService;
            this.feedService = feedService;
        }

        [HttpGet]
        public UserViewModel GetUser()
        {
            //var user = currentUserProvider.GetCurrentUser();
            var user = userService.GetByUserName(WebSecurity.CurrentUserName);

            var vm = new UserViewModel(user);
            foreach (Guid feedId in user.Feeds)
            {
                vm.Feeds.Add(new FeedViewModel(feedService.GetById(feedId)));
            }

            return vm; 
        }

        [HttpPost]
        [ActionName("category")]
        public void AddCategory([FromBody]string categoryName)
        {
            userService.AddCategory(categoryName);
        }

        [HttpDelete]
        [ActionName("category")]
        public void RemoveCategory(string id)
        {
            userService.RemoveCategory(id);
        }

        [HttpPost]
        [ActionName("feed")]
        public FeedViewModel SuscribeFeed([FromBody]string url)
        {
            var feed = feedService.Suscribe(url);
            feedService.RefreshFeed(feed.Id);

            return new FeedViewModel(feed);
        }

        [HttpDelete]
        [ActionName("feed")]
        public void UnsuscribeFeed(Guid id)
        {
            feedService.Unsuscribe(id);
        }
    }
}
