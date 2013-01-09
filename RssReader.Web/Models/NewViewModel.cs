using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using RssReader.Model;

namespace RssReader.Web.Models
{
    public class NewViewModel : BaseMappeableViewModel<New>
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string Linke { get; set; }

        public IList<string> Tags { get; set; }

        static NewViewModel()
        {
            Mapper.CreateMap<New, NewViewModel>();
            Mapper.CreateMap<NewViewModel, New>();
        }

        public NewViewModel(New newEntry)
        {
            Mapper.Map(newEntry, this);            
        }

        public NewViewModel()
        {
        }
    }
}