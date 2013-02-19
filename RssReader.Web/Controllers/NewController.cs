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

        [HttpPost]
        public void TagNew([FromBody]TagNewViewModel tagNew)
        {
            newService.TagNew(tagNew.NewId, tagNew.TagName);
        }

        [HttpPost]
        public void UnTagNew([FromBody]TagNewViewModel tagNew)
        {
            newService.UnTagNew(tagNew.NewId, tagNew.TagName);
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
