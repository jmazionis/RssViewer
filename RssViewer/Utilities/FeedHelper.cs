using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace RssViewer.Utilities
{
    public static class FeedHelper
    {
        private static string RssDataPath = LocateFeedData();

        public static IEnumerable<RssFeed> LoadFeeds(string xmlUri)
        {
            XDocument xml = XDocument.Load(xmlUri);
            var q = from c in xml.Descendants("Feed")
                    select new RssFeed
                    {
                        Url = (string) c.Element("Url"),
                        LastUpdated = (string) c.Element("LastUpdated")
                    };
            return q;
        }

        public static void UpdateTimes(RssFeed feed)
        {
            XDocument xml = XDocument.Load(RssDataPath);
            var feedElem = xml.Descendants("Feed").FirstOrDefault(x => x.Element("Url").Value == feed.Url);
            if (feedElem != null)
            {
                feedElem.Element("LastUpdated").Value = feed.LastUpdated;
            }
            xml.Save(RssDataPath);
        }

        public static void SaveFeeds(List<RssFeed> feedList)
        {
            XDocument xml = XDocument.Load(RssDataPath);
            XElement element = new XElement("Feeds");
            element.RemoveAll();
          
            XElement newXml = new XElement("Feeds",
                                        from f in feedList
                                        select new XElement("Feed",
                                                    new XElement("Url", f.Url),
                                                    new XElement("LastUpdated", f.LastUpdated))
                                       );
            newXml.Save(RssDataPath);
        }

        public static string LocateFeedData() //get RssData.xml file path
        {
            string projectPath = Environment.CurrentDirectory; 
            string rssDataPath = projectPath.Replace("\\bin\\Debug", "");
            rssDataPath += "\\RssData.xml";
            return rssDataPath;
        }
    }
}
