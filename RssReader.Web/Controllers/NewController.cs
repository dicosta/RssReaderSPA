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
    public class NewController : ApiController
    {
        private readonly INewService newService;
        private readonly ICurrentUserProvider currentUserProvider;

        public NewController(INewService newService, 
            ICurrentUserProvider currentUserProvider)
        {
            this.newService = newService;
            this.currentUserProvider = currentUserProvider;
        }

        public void TagNew(Guid newId, string categoryName)
        {
            newService.TagNew(newId, categoryName);
        }

        public void UnTagNew(Guid newId, string categoryName)
        {
            newService.UnTagNew(newId, categoryName);
        }

        public IList<NewViewModel> GetNews()
        {
            IList<NewViewModel> retValue = new List<NewViewModel>();

            foreach (var newEntry in newService.GetNews())
            {
                retValue.Add(new NewViewModel(newEntry));
            }

            return retValue;
        }

        public IList<NewViewModel> GetNewsByTag(string categoryName)
        {
            IList<NewViewModel> retValue = new List<NewViewModel>();

            foreach (var newEntry in newService.GetNewsByTag(categoryName))
            {
                retValue.Add(new NewViewModel(newEntry));
            }

            return retValue;
        }
    }
}
