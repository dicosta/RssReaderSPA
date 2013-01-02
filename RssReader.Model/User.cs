﻿using System;
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
    }
}
