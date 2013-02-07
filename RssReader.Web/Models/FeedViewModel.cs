using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using RssReader.Model;

namespace RssReader.Web.Models
{
    public class FeedViewModel : BaseMappeableViewModel<Feed>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string URL { get; set; }

        static FeedViewModel()
        {
            Mapper.CreateMap<Feed, FeedViewModel>();
            Mapper.CreateMap<FeedViewModel, Feed>();
        }

        public FeedViewModel(Feed feed)
        {
            Mapper.Map(feed, this);            
        }

        public FeedViewModel()
        {
        }
    }
}