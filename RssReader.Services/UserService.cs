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
    public class UserService : BaseService, IUserService
    {
        private readonly IGuidKeyedRepository<User> userRepository;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly INewService newService;

        public UserService(IGuidKeyedRepository<User> userRepository,
            ICurrentUserProvider currentUserProvider,
            INewService newService)
        {
            this.userRepository = userRepository;
            this.currentUserProvider = currentUserProvider;
            this.newService = newService;
        }

        public void AddCategory(string categoryName)
        {
            var user = currentUserProvider.GetCurrentUser();

            if (user.Tags.Contains(categoryName))
            {
                throw new RssReaderBusinessException("Category already exists.");
            }
            else
            {
                user.Tags.Add(categoryName);
                userRepository.Update(user);

                //add category to bayesian classifier...

                logger.Info("added category {0} for User {1}", categoryName, user.UserName);
            }
        }

        public void RemoveCategory(string categoryName)
        {
            var user = currentUserProvider.GetCurrentUser();

            if (user.Tags.Contains(categoryName))
            {
                //remove category from bayesian classifier...
             
                //untag news
                foreach (var matchedNew in newService.GetNewsByTag(categoryName).Select(n => n.Id))
                {
                    newService.UnTagNew(matchedNew, categoryName);
                }

                user.Tags.Remove(categoryName);
                userRepository.Update(user);

                logger.Info("removed category {0} for User {1}", categoryName, user.UserName);
            }
            else
            {
                throw new RssReaderBusinessException("Category does not exist.");
            }
        }
    }
}
