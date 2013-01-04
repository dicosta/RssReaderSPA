using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model
{
    public class New
    {
        public Guid Id { get; set; }

        public Guid FeedId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Link { get; set; }

        public DateTime TimeStamp { get; set; } 

        public IList<string> Tags { get; set; }
    }
}
