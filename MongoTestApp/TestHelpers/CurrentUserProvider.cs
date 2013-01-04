using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;
using RssReader.Model.Contracts;
using RssReader.Services.Contracts;

namespace MongoTestApp.TestHelpers
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IGuidKeyedRepository<User> userRepository;

        public CurrentUserProvider(IGuidKeyedRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public RssReader.Model.User GetCurrentUser()
        {
            return userRepository.GetAll().First();            
        }
    }
}
