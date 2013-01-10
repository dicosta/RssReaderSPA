using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace RssReader.Model
{
    public interface IGuidKeyedEntity
    {
        Guid Id { get; set; }
    }
}
