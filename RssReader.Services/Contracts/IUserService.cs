using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;

namespace RssReader.Services.Contracts
{
    public interface IUserService
    {
        void AddCategory(string categoryName);

        void RemoveCategory(string categoryName);

        bool UserIsEnabled(string userName);

        User GetByUserName(string userName);

        User GetById(Guid userId);

        User GetByConfirmationToken(string confirmationToken);

        User GetByPasswordResetToken(string passwrodResetToken);

        IQueryable<User> GetAll();

        void Update(User user);

        void Create(User user);

        void Delete(Guid userId);
    }
}
