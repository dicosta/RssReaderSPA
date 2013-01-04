using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace RssReader.Services
{
    public class BaseService
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
