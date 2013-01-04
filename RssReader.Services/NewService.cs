using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;
using RssReader.Model.Contracts;
using RssReader.Services.Contracts;
using RssReader.Services.Exceptions;

namespace RssReader.Services
{
    public class NewService : BaseService, INewService
    {
        private readonly IGuidKeyedRepository<New> newRepository;
        private readonly ICurrentUserProvider currentUserProvider;

        public NewService(IGuidKeyedRepository<New> newRepository,
                        ICurrentUserProvider currentUserProvider)
        {
            this.newRepository = newRepository;
            this.currentUserProvider = currentUserProvider;
        }

        public void TagNew(Guid newId, string categoryName)
        {
            var user = currentUserProvider.GetCurrentUser();

            if (user.Tags.Contains(categoryName))
            {
                var entry = newRepository.GetById(newId);
                if (!entry.Tags.Contains(categoryName))
                {
                    entry.Tags.Add(categoryName);
                    newRepository.Update(entry);

                    //train bayesian classifier...

                    logger.Info("User {0} successfully tagged new {1} with category {2}", user.UserName, entry.Id, categoryName);
                }
                else
                {
                    throw new RssReaderBusinessException("Entry already has this category.");
                }
            }
            else
            {
                throw new RssReaderBusinessException("Category does not exist.");
            }
        }

        public void UnTagNew(Guid newId, string categoryName)
        {
            var user = currentUserProvider.GetCurrentUser();

            if (user.Tags.Contains(categoryName))
            {
                var entry = newRepository.GetById(newId);
                if (entry.Tags.Contains(categoryName))
                {
                    entry.Tags.Remove(categoryName);
                    newRepository.Update(entry);

                    //train bayesian classifier...

                    logger.Info("User {0} successfully untagged new {1} from category {2}", user.UserName, entry.Id, categoryName);
                }
                else
                {
                    throw new RssReaderBusinessException("Entry does not belongs to this category.");
                }
            }
            else
            {
                throw new RssReaderBusinessException("Category does not exist.");
            }
        }


        public IQueryable<New> GetNewsByTag(string categoryName)
        {
            var userId = currentUserProvider.GetCurrentUser().Id;

            return newRepository.GetAll()
                .Where(n => n.UserId == userId && n.Tags.Contains(categoryName))
                .OrderBy(n => n.TimeStamp);
        }

        public IQueryable<New> GetNews()
        {
            var userId = currentUserProvider.GetCurrentUser().Id;

            return newRepository.GetAll()
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.TimeStamp); 
        }
    }
}
