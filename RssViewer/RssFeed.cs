using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace RssViewer
{
    public class RssFeed
    {
        public string Url { get; set; }
        public string LastUpdated { get; set; }
        public int NewItemsCount { get; set; }

        public RssFeed() {}

        public RssFeed(string url, string lastUpdated)
        {
            this.Url = url;
            this.LastUpdated = lastUpdated;
            this.NewItemsCount = 0;
        }

        public void RefreshNewItemsCount(SyndicationFeed feed)
        {
            DateTime lastUpdatedDateTime = DateTime.Parse(this.LastUpdated);
            int count = 0;
            foreach (var feedItem in feed.Items)
            {
                if (feedItem.PublishDate > lastUpdatedDateTime)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            this.LastUpdated = feed.Items.ElementAt(0).PublishDate.ToString();
            this.NewItemsCount = count;
        }

    }
}
