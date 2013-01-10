using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using RssReader.Model;

namespace RssReader.Web.Models
{
    public abstract class BaseMappeableViewModel<T> where T : IGuidKeyedEntity
    {
        public Guid Id { get; set; }

        public T MapFromViewModel()
        {
            return Mapper.Map<T>(this);
        }
    }
}