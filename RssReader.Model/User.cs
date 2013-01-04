using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public IList<string> Tags { get; set; }

        public IList<Guid> Feeds { get; set; }

        public User()
        {
            this.Tags = new List<string>();
            this.Feeds = new List<Guid>();
        }
    }
}
