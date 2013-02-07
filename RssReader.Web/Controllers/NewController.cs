using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
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

        [HttpGet]
        public IList<NewViewModel> GetNews()
        {
            IList<NewViewModel> retValue = new List<NewViewModel>();

            foreach (var newEntry in newService.GetNews())
            {
                retValue.Add(new NewViewModel(newEntry));
            }

            return retValue;
        }

        [HttpGet]
        public IList<NewViewModel> GetNewsByTag(string id)
        {
            IList<NewViewModel> retValue = new List<NewViewModel>();

            foreach (var newEntry in newService.GetNewsByTag(id))
            {
                retValue.Add(new NewViewModel(newEntry));
            }

            return retValue;
        }
    }
}
