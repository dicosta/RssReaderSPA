using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using RssReader.Model;

namespace RssReader.Web.Models
{
    public class UserViewModel : BaseMappeableViewModel<User>
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public IList<string> Tags { get; set; }

        public IList<FeedViewModel> Feeds { get; set; }

        static UserViewModel()
        {
            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Feeds, opt => opt.Ignore());

            Mapper.CreateMap<UserViewModel, User>();
        }
                
        public UserViewModel(User user)
        {
            Mapper.Map(user, this);            
        }

        public UserViewModel()
        {
            Feeds = new List<FeedViewModel>();
        }
    }
}