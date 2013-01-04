﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;

namespace RssReader.Services.Contracts
{
    public interface IFeedService
    {
        void SuscribeFeed(string feedURL);

        void RefreshFeed(Guid feedId);
    }
}
