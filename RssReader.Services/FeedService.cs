using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RssReader.Model;
using RssReader.Model.Contracts;

namespace RssReader.Services
{
    public class FeedService
    {
        private readonly IGuidKeyedRepository<Feed> feedRepository;
        private readonly IGuidKeyedRepository<New> newRepository;

        public FeedService(IGuidKeyedRepository<Feed> feedRepository,
            IGuidKeyedRepository<New> newRepository)
        {
            this.feedRepository = feedRepository;
        }

        public void SuscribeFeed(Feed newFeed)
        {
            using (XmlReader xml = XmlReader.Create(newFeed.URL))
            {
                var feed = SyndicationFeed.Load(xml);

                newFeed.Title = feed.Title.Text;
                newFeed.Description = feed.Description.Text;
                newFeed.ImageURL = feed.ImageUrl.ToString();
                newFeed.LastItemTimeStamp = DateTime.UtcNow.AddYears(-5);
            }

            //throw if error reading feed.

            //add feed to collection
            feedRepository.Add(newFeed);

            //update current user feed collection with feed id.
        }

        public void RefreshFeed(Guid feedId)
        {
            var feed = feedRepository.GetById(feedId);

            using (XmlReader xml = XmlReader.Create(feed.URL))
            {
                //fetches the updated feed.
                var updatedFeed = SyndicationFeed.Load(xml);

                //get the new items 
                var newItems = updatedFeed.Items.Where(i => i.PublishDate.UtcDateTime > feed.LastItemTimeStamp)
                    .OrderBy(i => i.PublishDate.UtcDateTime);

                if (newItems.Any())
                {
                    feed.LastItemTimeStamp = newItems.Last().PublishDate.UtcDateTime;
                    feedRepository.Update(feed);

                    foreach (var newItem in newItems)
                    {
                        newRepository.Add(new New()                        
                        {
                            Title = newItem.Title.Text,
                            FeedId = feed.Id,
                            Tags = new List<string>(), //get them from classify
                            Body = newItem.Content.ToString()                            
                        });
                    }
                }
            }
        }
    }
}
