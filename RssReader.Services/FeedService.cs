﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RssReader.Model;
using RssReader.Model.Contracts;
using RssReader.Services.Contracts;
using RssReader.Services.Helpers;
using Argotic.Syndication;
using System.Security.Cryptography;

namespace RssReader.Services
{
    public class FeedService : BaseService, IFeedService
    {
        private readonly IGuidKeyedRepository<Feed> feedRepository;
        private readonly IGuidKeyedRepository<New> newRepository;
        private readonly IGuidKeyedRepository<User> userRepository;
        private readonly ICurrentUserNameProvider currentUserNameProvider;

        public FeedService(IGuidKeyedRepository<Feed> feedRepository,
            IGuidKeyedRepository<New> newRepository,
            ICurrentUserNameProvider currentUserNameProvider,
            IGuidKeyedRepository<User> userRepository)
        {
            this.feedRepository = feedRepository;
            this.newRepository = newRepository;
            this.currentUserNameProvider = currentUserNameProvider;
            this.userRepository = userRepository;
        }

        public Feed GetById(Guid feedId)
        {
            return feedRepository.GetById(feedId);
        }
        
        public Feed Suscribe(string feedURL)
        {
            //check already suscribed...

            GenericSyndicationFeed feed = GenericSyndicationFeed.Create(new Uri(feedURL));

            var newFeed = new Feed();

            newFeed.URL = feedURL;
            newFeed.Title = feed.Title;
            newFeed.Description = feed.Description;
           
            //throw if error reading feed.

            //add feed to collection
            feedRepository.Add(newFeed);

            //update current user feed collection with feed id.
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();
            user.Feeds.Add(newFeed.Id);
            userRepository.Update(user);

            logger.Info("User {0} successfully suscribed to feed {1}", user.Username, newFeed.URL);

            return newFeed;
        }

        public void Unsuscribe(Guid feedId)
        {
            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();
            user.Feeds.Remove(feedId);
            userRepository.Update(user);

            feedRepository.Delete(feedId);

            var newsToDelete = newRepository.GetAll()
                .Where(n => n.FeedId == feedId)
                .Select(n => n.Id);

            foreach (var newId in newsToDelete)
            {
                newRepository.Delete(newId);
            }

            //should we keep the data in the probability matrix?

            logger.Info("User {0} successfully unsuscribed from feed {1}", user.Username, feedId);
        }

        public void RefreshFeed(Guid feedId)
        {
            var feed = feedRepository.GetById(feedId);

            logger.Debug("refreshing feed {0}", feed.Title);

            GenericSyndicationFeed updatedFeed = GenericSyndicationFeed.Create(new Uri(feed.URL));

            var user = userRepository.GetAll().Where(u => u.Username == currentUserNameProvider.GetCurrentUserName()).FirstOrDefault();
            bool updateFeedEntity = true;
            
            foreach (GenericSyndicationItem item in updatedFeed.Items)
            {
                string itemDigest = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(item.Summary)));

                if (string.IsNullOrEmpty(feed.LastItemDigest) || (itemDigest != feed.LastItemDigest))
                {
                    logger.Debug("found new entry for feed {0}", feed.Title);

                    newRepository.Add(new New()
                    {
                        Title = item.Title,
                        FeedId = feed.Id,
                        UserId = user.Id,
                        Tags = new List<string>(), //get them from classify
                        Body = item.Summary
                    });

                    if (updateFeedEntity)
                    {
                        feed.LastItemDigest = itemDigest;
                        feedRepository.Update(feed);

                        updateFeedEntity = false;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
