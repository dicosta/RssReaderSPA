using System;
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

        public FeedService(IGuidKeyedRepository<Feed> feedRepository,
            IGuidKeyedRepository<New> newRepository)
        {
            this.feedRepository = feedRepository;
            this.newRepository = newRepository;
        }
        
        public void SuscribeFeed(Feed newFeed)
        {
            GenericSyndicationFeed feed = GenericSyndicationFeed.Create(new Uri(newFeed.URL));

            newFeed.Title = feed.Title;
            newFeed.Description = feed.Description;
            //newFeed.ImageURL = feed. ImageUrl.ToString();
            //newFeed.LastItemDigest = "0";
            
            /*
            WebClient client = new WebClient();
            using (XmlReader xml = new SyndicationFeedXmlReader(client.OpenRead(newFeed.URL)))
            {
                var feed = SyndicationFeed.Load(xml);

                newFeed.Title = feed.Title.Text;
                newFeed.Description = feed.Description.Text;
                newFeed.ImageURL = feed.ImageUrl.ToString();
                newFeed.LastItemTimeStamp = DateTime.UtcNow.AddYears(-5);
            }
            */

            //throw if error reading feed.

            //add feed to collection
            feedRepository.Add(newFeed);

            //update current user feed collection with feed id.
        }

        public void RefreshFeed(Guid feedId)
        {
            var feed = feedRepository.GetById(feedId);

            //using (XmlReader xml = XmlReader.Create(feed.URL))
            //{
                //fetches the updated feed.
                //var updatedFeed = SyndicationFeed.Load(xml);
                GenericSyndicationFeed updatedFeed = GenericSyndicationFeed.Create(new Uri(feed.URL));

                bool updateFeedEntity = true;
                foreach (GenericSyndicationItem item in updatedFeed.Items)
                {
                    string itemDigest = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(item.Summary)));

                    if (string.IsNullOrEmpty(feed.LastItemDigest) || (itemDigest != feed.LastItemDigest))
                    {
                        newRepository.Add(new New()
                        {
                            Title = item.Title,
                            FeedId = feed.Id,
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


                //get the new items 
                /*
                var newItems = updatedFeed.Items.Where(i => i.PublishedOn.ToLocalTime() > feed.LastItemTimeStamp)
                    .OrderBy(i => i.PublishedOn.ToLocalTime());

                if (newItems.Any())
                {
                    feed.LastItemTimeStamp = newItems.Last().PublishedOn.ToLocalTime();
                    feedRepository.Update(feed);

                    foreach (var newItem in newItems)
                    {
                        newRepository.Add(new New()                        
                        {
                            Title = newItem.Title,
                            FeedId = feed.Id,
                            Tags = new List<string>(), //get them from classify
                            Body = newItem.Summary.ToString()                            
                        });
                    }
                }
                */
            //}
        }
    }
}
