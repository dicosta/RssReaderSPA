using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Services.Exceptions
{
    public class RssReaderBusinessException : Exception
    {
        public RssReaderBusinessException(string message)
            : base(message)
        {
        }
    }
}
