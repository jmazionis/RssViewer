using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace RssViewer
{
    public partial class NewChannelDialog : Window
    {
        private MWindow mainWindow;

        public NewChannelDialog(MWindow window)
        {
            InitializeComponent();
            this.mainWindow = window;
        }

        private void addFeedButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var feedUrl = urlTextBox.Text;
            bool showInvalidMsg = false;
            SyndicationFeed rssFeedChannel;
            try
            {
                using (var xmlReader = XmlReader.Create(feedUrl))
                {
                    rssFeedChannel = SyndicationFeed.Load(xmlReader);
                }
                if (!mainWindow.feedList.Any(x => feedUrl == x.Url))
                {
                    this.mainWindow.channelList.Add(rssFeedChannel);
                    var rssFeed = new RssFeed(feedUrl, rssFeedChannel.Items.ElementAt(0).PublishDate.ToString());
                    this.mainWindow.feedList.Add(rssFeed);
                    rssFeed.RefreshNewItemsCount(rssFeedChannel);
                    this.mainWindow.AddChannelPanel(rssFeedChannel.Description.Text + "(" + rssFeed.NewItemsCount + ")", this.mainWindow.channelList.Count - 1);
                    if (!showInvalidMsg)
                    {
                        this.validationBlock.Text = "";
                    }
                    this.validationBlock.Foreground = Brushes.LightGreen;
                    this.validationBlock.Text = "New feed was successfully added to the list.";
                }
                else
                {
                    this.validationBlock.Foreground = Brushes.LightSalmon;
                    this.validationBlock.Text = "Channel with specified URL already exists.";
                }
            }
            catch (Exception)
            {
                this.validationBlock.Foreground = Brushes.LightSalmon;
                this.validationBlock.Text = "Unable to find feed at specified URL.";
            }
        }
    }
}
