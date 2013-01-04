using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Services.Contracts
{
    public interface IUserService
    {
        void AddCategory(string categoryName);

        void RemoveCategory(string categoryName);
    }
}
