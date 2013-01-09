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
        private readonly ICurrentUserIdProvider currentUserIdProvider;
        private readonly INewService newService;

        public UserService(IGuidKeyedRepository<User> userRepository,
            ICurrentUserIdProvider currentUserIdProvider,
            INewService newService)
        {
            this.userRepository = userRepository;
            this.currentUserIdProvider = currentUserIdProvider;
            this.newService = newService;
        }

        public void AddCategory(string categoryName)
        {            
            var user = this.GetById(currentUserIdProvider.GetCurrentUserId());

            if (user.Tags.Contains(categoryName))
            {
                throw new RssReaderBusinessException("Category already exists.");
            }
            else
            {
                user.Tags.Add(categoryName);
                userRepository.Update(user);

                //add category to bayesian classifier...

                logger.Info("added category {0} for User {1}", categoryName, user.Username);
            }
        }

        public void RemoveCategory(string categoryName)
        {
            var user = this.GetById(currentUserIdProvider.GetCurrentUserId());

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

                logger.Info("removed category {0} for User {1}", categoryName, user.Username);
            }
            else
            {
                throw new RssReaderBusinessException("Category does not exist.");
            }
        }
        
        public bool UserIsEnabled(string userName)
        {
            return userRepository.GetAll().Where(u => u.Username == userName).Any();
        }

        public User GetByUserName(string userName)
        {
            return userRepository.GetAll().Where(u => u.Username == userName).FirstOrDefault();
        }

        public User GetById(Guid userId)
        {
            return userRepository.GetById(userId);
        }

        public User GetByConfirmationToken(string confirmationToken)
        {
            return userRepository.GetAll().Where(u => u.ConfirmationToken == confirmationToken).FirstOrDefault();
        }

        public User GetByPasswordResetToken(string passwordResetToken)
        {
            return userRepository.GetAll().Where(u => u.PasswordResetToken == passwordResetToken).FirstOrDefault();
        }

        public void Update(User user)
        {
            userRepository.Update(user);
        }

        public void Create(User user)
        {
            userRepository.Add(user);
        }

        public void Delete(Guid userId)
        {
            userRepository.Delete(userId);
        }

        public IQueryable<User> GetAll()
        {
            return userRepository.GetAll();
        }
    }
}
