using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RssViewer.BackgroundWorkers;
using RssViewer.Utilities;

namespace RssViewer
{
    public partial class MWindow : Window
    {
        private WebBrowser webBrowser;
        private Loader loader;
        private Refresher refresher;
        public List<RssFeed> feedList { get; set; }
        public IList<SyndicationFeed> channelList { get; set; }
        public RssChannelStackPanel rightClickedRssPanel { get; set; }

        public MWindow()
        {
            InitializeComponent();
            this.InitializeWebBrowser();
            this.InitializeButtons();
            loader = new Loader(this);
            loader.InitializeChannels();
            refresher = new Refresher(this);
            this.Closing += new CancelEventHandler(MWindow_Closing);
        }

        private void MWindow_Closing(Object sender, CancelEventArgs e)
        {
            FeedHelper.SaveFeeds(this.feedList);
        }

        public void AddChannelPanel(string channelTitle, int id)
        {
            RssChannelStackPanel rssStackPanel = new RssChannelStackPanel(id);
            rssStackPanel.Orientation = Orientation.Horizontal;
            rssStackPanel.Margin = new Thickness(10, 2, 0, 2);
            rssStackPanel.MouseEnter += new MouseEventHandler(RssChannelItem_MouseEnter);
            rssStackPanel.MouseLeave += new MouseEventHandler(RssChannelItem_MouseLeave);
            rssStackPanel.MouseLeftButtonDown += new MouseButtonEventHandler(RssChannelItem_MouseLeftButtonDown);
            rssStackPanel.MouseRightButtonDown += new MouseButtonEventHandler(RssChannelItem_MouseRightButtonDown);

            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("/Pictures/rssIcon.png", UriKind.Relative);
            bitmapImage.EndInit();
            image.Source = bitmapImage;
            image.Width = 20;
            image.Height = 20;
            rssStackPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Name = "channelItem";
            textBlock.Foreground = Brushes.White;
            textBlock.FontSize = 14.0;
            textBlock.Text = channelTitle;
            textBlock.Margin = new Thickness(5, 0, 0, 0);
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            rssStackPanel.Children.Add(textBlock);

            ContextMenu contextMenu = new ContextMenu();
            rssStackPanel.ContextMenu = contextMenu;

            MenuItem removeChannelMenuItem = new MenuItem();
            removeChannelMenuItem.Click += new RoutedEventHandler(removeChannelMenuItem_Click);
            removeChannelMenuItem.Header = "Remove channel";
            contextMenu.Items.Add(removeChannelMenuItem);

            channelItemPanel.Children.Add(rssStackPanel);
        }

        private void removeChannelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            channelItemPanel.Children.Remove(this.rightClickedRssPanel);
            this.feedList.RemoveAt(this.rightClickedRssPanel.Id);
        }

        private void RssChannelItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.rightClickedRssPanel = (RssChannelStackPanel) sender;
        }

        private void RssChannelItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rssChannelStackPanel = (RssChannelStackPanel) sender;
            webBrowser.NavigateToString(HtmlBuilder.ConstructItemsHtml(this.channelList[rssChannelStackPanel.Id])); 
        }

        private void RssChannelItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var stackPanel = (StackPanel) sender;
            stackPanel.Background = Brushes.Brown;
            stackPanel.Cursor = Cursors.Hand;
        }

        private void RssChannelItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var stackPanel = (StackPanel) sender;
            stackPanel.Background = Brushes.Black;
            stackPanel.Cursor = Cursors.Arrow;
        }

        private void InitializeWebBrowser()
        {
            this.webBrowser = new WebBrowser();
            this.webBrowserPanel.Children.Add(webBrowser);
        }

        private void InitializeButtons()
        {
            refreshTextBlock.MouseLeftButtonDown += new MouseButtonEventHandler(refreshTextBlock_MouseLeftButtonDown);
            newChannelTextBlock.MouseLeftButtonDown += new MouseButtonEventHandler(newChannelTextBlock_MouseLeftButtonDown);
        }

        void newChannelTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new NewChannelDialog(this);
            dlg.ShowDialog();
        }

        void refreshTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            refresher.RefreshAllFeeds();
        }

    }
}
