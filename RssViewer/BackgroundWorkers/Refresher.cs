using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using RssViewer.Utilities;
using System.Windows.Media.Animation;

namespace RssViewer.BackgroundWorkers
{
    public class Refresher
    {
        private MWindow mWindow;
        private SyndicationFeed rssFeedChannel;

        public Refresher(MWindow mWindow)
        {
            this.mWindow = mWindow;   
        }
         public void RefreshAllFeeds()
         {
            BackgroundWorker refreshWorker = new BackgroundWorker();
            refreshWorker.WorkerReportsProgress = true;
            refreshWorker.DoWork += this.refreshWorker_DoWork;
            refreshWorker.RunWorkerCompleted += this.refreshWorker_RunWorkerCompleted;
            refreshWorker.ProgressChanged += this.refreshWorker_ProgressChanged;
            refreshWorker.RunWorkerAsync();
        }

        private void refreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action changeStatusState = () => { this.mWindow.statusMessageBox.Text = "Refreshing..."; this.mWindow.progressBar.Visibility = Visibility.Visible; };
            this.mWindow.statusMessageBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, changeStatusState);
            BackgroundWorker refreshWorker = sender as BackgroundWorker;
            for (int i = 0; i < this.mWindow.feedList.Count; i++)
            {
                Thread.Sleep(200);
                using (var xmlReader = XmlReader.Create(this.mWindow.feedList[i].Url))
                {
                    this.rssFeedChannel = SyndicationFeed.Load(xmlReader);
                }
                this.mWindow.channelList[i] = rssFeedChannel;
                this.mWindow.feedList[i].RefreshNewItemsCount(this.mWindow.channelList.ElementAt(i));
                refreshWorker.ReportProgress((int) (((double) (i + 1) / (double) this.mWindow.feedList.Count) * 100));
            }
        }

        private void refreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) //Update UI when all feeds were loaded
        {
            mWindow.statusMessageBox.Visibility = Visibility.Visible;
            mWindow.progressBar.Visibility = Visibility.Visible;
            for (int i = 0; i < this.mWindow.feedList.Count; i++)
            {
                RssChannelStackPanel pan = (RssChannelStackPanel) this.mWindow.channelItemPanel.Children[i + 1];
                TextBlock tb = (TextBlock) pan.Children[1];
                tb.Text = this.mWindow.channelList[i].Title.Text + " (" + this.mWindow.feedList[i].NewItemsCount + ")";
            }
            this.mWindow.progressBar.Visibility = Visibility.Hidden;
            this.mWindow.statusMessageBox.Text = "";
            this.mWindow.progressBar.Value = 0.0;
        }

        private void refreshWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.mWindow.progressBar.Value = e.ProgressPercentage;
        }
    }
}
