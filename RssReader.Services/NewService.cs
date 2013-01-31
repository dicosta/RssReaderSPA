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
        private readonly ICurrentUserNameProvider currentUserNameProvider;
        private readonly IGuidKeyedRepository<User> userRepository;
        
        public NewService(IGuidKeyedRepository<New> newRepository,
                        ICurrentUserProvider currentUserProvider,
                        ICurrentUserNameProvider currentUserNameProvider,
                        IGuidKeyedRepository<User> userRepository)
        {
            this.newRepository = newRepository;
            this.currentUserNameProvider = currentUserNameProvider;
            this.userRepository = userRepository;
        }

        public void TagNew(Guid newId, string categoryName)
        {
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();

            if (user.Tags.Contains(categoryName))
            {
                var entry = newRepository.GetById(newId);
                if (!entry.Tags.Contains(categoryName))
                {
                    entry.Tags.Add(categoryName);
                    newRepository.Update(entry);

                    //train bayesian classifier...

                    logger.Info("User {0} successfully tagged new {1} with category {2}", user.Username, entry.Id, categoryName);
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
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();

            if (user.Tags.Contains(categoryName))
            {
                var entry = newRepository.GetById(newId);
                if (entry.Tags.Contains(categoryName))
                {
                    entry.Tags.Remove(categoryName);
                    newRepository.Update(entry);

                    //train bayesian classifier...

                    logger.Info("User {0} successfully untagged new {1} from category {2}", user.Username, entry.Id, categoryName);
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
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();

            return newRepository.GetAll()
                .Where(n => n.UserId == user.Id && n.Tags.Contains(categoryName))
                .OrderBy(n => n.TimeStamp);
        }

        public IQueryable<New> GetNews()
        {
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();

            return newRepository.GetAll()
                .Where(n => n.UserId == user.Id)
                .OrderBy(n => n.TimeStamp); 
        }
    }
}
